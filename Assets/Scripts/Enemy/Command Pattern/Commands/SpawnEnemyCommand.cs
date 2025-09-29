using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyCommand : EnemyCommand
{
    private List<EnemyData> enemyData = new();
    private Vector2 position;
    public SpawnEnemyCommand(EnemyData enemyData, int count, Vector2 position)
    {
        for (int i = 0; i < count; i++)
        {
            this.enemyData.Add(enemyData);
        }
        this.position = position;
    }
    public SpawnEnemyCommand(List<EnemyData> enemyData, Vector2 position)
    {
        this.position = position;
        this.enemyData = enemyData;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        EnemyManager.Instance.SpawnEnemyBurst(enemyData.ToArray(), position);
        yield break;
    }
}

