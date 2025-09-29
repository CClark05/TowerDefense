using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDeadEnemiesCommand : EnemyCommand
{
    private int enemyCount;
    private List<EnemyData> deadEnemies;
    private float pollingInterval;
    public WaitForDeadEnemiesCommand(int enemyCount, List<EnemyData> deadEnemies, float pollingInterval = 0.1f)
    {
        this.enemyCount = enemyCount;
        this.deadEnemies = deadEnemies;
        this.pollingInterval = pollingInterval;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        while (true)
        {
            deadEnemies.Clear();
            deadEnemies.AddRange(EnemyManager.Instance.DeadEnemies);
            if(deadEnemies.Count >= enemyCount)
                yield break;
            yield return new WaitForSeconds(pollingInterval);
        }
        
    }
}