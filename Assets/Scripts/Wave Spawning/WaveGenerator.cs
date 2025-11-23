using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct EnemySpawn
{
    public EnemyCost enemy;
    public float delay;
}

public struct WavePlan
{
    public int waveIndex;
    public bool isBoss;
    public bool isSpike;
    public List<EnemySpawn> enemySpawns;
}

public class WaveGenerator
{
    private WaveSettings waveSettings;

    public WaveGenerator(WaveSettings settings)
    {
        waveSettings = settings;
    }

    public WavePlan Generate(int wave)
    {
        var budgetTuning = new BudgetTuning();
        float smooth = waveSettings.BaseBudget * Mathf.Pow(waveSettings.GrowthRate, wave - 1) + waveSettings.AdditivePerWave * (wave - 1);
        float noise = 1f + (UnityEngine.Random.value * 2f - 1f) * waveSettings.Variance;
        float budget = smooth * noise;
        bool isBoss = (wave % waveSettings.BossEvery == 0);
        bool isSpike = wave > waveSettings.BossEvery && (wave - 1) % waveSettings.BossEvery == 0;
        if (isBoss) budget *= waveSettings.BossMultiplier;
        if(isSpike) budget *= waveSettings.SpikeAfterBoss;
        Debug.Log("Budget : " + budget);
        var pool = waveSettings.Enemies.Where(e => e.MinWave <= wave && (e.MaxWave == 0 || wave <= e.MaxWave) && (isBoss ? (e.EnemyData.IsBoss || !e.EnemyData.IsBoss) : !e.EnemyData.IsBoss)).
            Where(e => CalculateCost(e, budgetTuning) <= budget).OrderBy(e => UnityEngine.Random.value).ToArray();
        var plan = new WavePlan
        {
            waveIndex = wave,
            isBoss = isBoss,
            isSpike = isSpike,
            enemySpawns = new List<EnemySpawn>()
        };
        if (isBoss)
        {
            var bosses = pool.Where(e => e.EnemyData.IsBoss).ToArray();
            if (!bosses.Any()) Debug.LogError("NO BOSS FOUND");
            var boss = bosses[UnityEngine.Random.Range(0, bosses.Length)];
            plan.enemySpawns.Add(new EnemySpawn { enemy = boss, delay = 1f });
            budget -= CalculateCost(boss, budgetTuning);
        }
        
        int attempts = 0;
        while (budget > 0 && !isBoss)
        {
            var enemiesNeeded = Mathf.Max(waveSettings.minEnemies - plan.enemySpawns.Count, 1);
            var budgetPerEnemy = budget / enemiesNeeded;
            var affordableEnemies = pool.Where(e => CalculateCost(e, budgetTuning) <= budgetPerEnemy).ToArray();
            while (affordableEnemies.Length == 0 && attempts < 50)
            {
                Debug.LogWarning(" No affordable enemies for budget per enemy " + budgetPerEnemy + ", increasing budget per enemy");
                budgetPerEnemy *= 1.1f;
                affordableEnemies = pool.Where(e => CalculateCost(e, budgetTuning) <= budgetPerEnemy).ToArray();
                attempts++;
            }
            if (affordableEnemies.Length == 0)
            {
                Debug.LogError(" No affordable enemies found even after attempts");
                break;
            }
            var enemy = affordableEnemies[UnityEngine.Random.Range(0, affordableEnemies.Length)];
            var cost = CalculateCost(enemy, budgetTuning);
            budget -= cost;
            Debug.Log(" Selected enemy " + enemy.name + " cost : " + cost);
            plan.enemySpawns.Add(new EnemySpawn { enemy = enemy, delay = UnityEngine.Random.Range(0.5f, 1f) });
        }
        return plan;
    }

    private int CalculateCost(EnemyCost enemyCost, BudgetTuning tuning)
    {
        return EnemyBudget.Cost(enemyCost.EnemyData.health, enemyCost.EnemyData.shields, enemyCost.EnemyData.speed, tuning, enemyCost.EnemyData.IsElite) + enemyCost.Weight;
    }
}