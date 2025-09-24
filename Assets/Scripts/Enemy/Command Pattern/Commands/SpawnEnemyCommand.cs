using System.Collections;
using UnityEngine;

public class SpawnEnemyCommand : EnemyCommand
{
    private EnemyData enemyData;
    private int count;
    private Vector2 position;
    public SpawnEnemyCommand(EnemyData enemyData, int count, Vector2 position)
    {
        this.enemyData = enemyData;
        this.count = count;
        this.position = position;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        EnemyManager.Instance.SpawnEnemyBurst(enemyData, count, position);
        yield break;
    }
}
