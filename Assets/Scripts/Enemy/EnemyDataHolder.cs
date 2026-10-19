using System;
using System.Security.Permissions;
using UnityEngine;

public class EnemyDataHolder : MonoBehaviour
{
    private EnemyData data;
    public EnemyData Data => data;
    public int MaxHP { get; private set; }
    public float SpeedIncrease { get; private set; }
    private EnemyCost[] enemiesUnderneath;
    public EnemyCost[] EnemiesUnderneath => enemiesUnderneath;
    public bool IsElite { get; private set; }
    public void Init(EnemyData data, bool isElite = false, EnemyCost[] enemiesUnderneath = null)
    {
        this.data = data;
        IsElite = isElite;
        if (isElite)
        {
            this.data = data.CloneRuntime();
            this.data.health = Mathf.CeilToInt(this.data.health * data.EliteBonus.healthMultiplier);
            this.data.speed *= data.EliteBonus.speedMultiplier;
        }
        int cycle = (EnemyManager.Instance.CurrentWave- 1) / EncounterGenerator.Instance.CycleLength;
        float mult = 1f + cycle * EnemyManager.Instance.WaveSettings.HpIncreaseAfterBoss;
        MaxHP = Mathf.CeilToInt(this.data.health * mult);
        SpeedIncrease = cycle * EnemyManager.Instance.WaveSettings.SpeedIncreaseAfterBoss;
        this.enemiesUnderneath = enemiesUnderneath;
    }
    
}