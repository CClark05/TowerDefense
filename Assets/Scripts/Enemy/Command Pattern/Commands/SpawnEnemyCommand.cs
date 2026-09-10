using System.Collections;
using UnityEngine;


public class SpawnEnemyCommand : EnemyCommand
{
    private EnemyData enemyData;
    private Vector2 position;
    public SpawnEnemyCommand(EnemyData enemyData, Vector2 position)
    {
        this.enemyData = enemyData;
        this.position = position;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        EnemyManager.Instance.SpawnEnemyAtPosition(enemyData, position);
        yield break;
    }
}