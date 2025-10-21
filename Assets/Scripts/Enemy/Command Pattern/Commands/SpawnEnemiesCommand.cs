using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesCommand : EnemyCommand
{
    private List<EnemyData> enemyData = new();
    private Vector2 position;
    private float delayBetweenSpawns;
    public SpawnEnemiesCommand(EnemyData enemyData, int count, Vector2 position, float delayBetweenSpawns)
    {
        for (int i = 0; i < count; i++)
        {
            this.enemyData.Add(enemyData);
        }
        this.position = position;
        this.delayBetweenSpawns = delayBetweenSpawns;
    }
    public SpawnEnemiesCommand(List<EnemyData> enemyData, Vector2 position, float delayBetweenSpawns)
    {
        this.position = position;
        this.enemyData = enemyData;
        this.delayBetweenSpawns = delayBetweenSpawns;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        foreach (var enemy in enemyData)
        {
            EnemyManager.Instance.SpawnEnemyAtPosition(enemy, position);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }
}