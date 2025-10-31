using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GetDeadEnemiesCommand : EnemyCommand
{
    private List<EnemyData> deadEnemies;
    private int count;
    public GetDeadEnemiesCommand(List<EnemyData> deadEnemies, int count)
    {
        this.deadEnemies = deadEnemies;
        this.count = count;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        deadEnemies.Clear();
        var pool = EnemyManager.Instance.DeadEnemies.Where(e => e.summonable).ToList();
        for (int i = 0; i < count; i++)
        {
            deadEnemies.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
        }
        yield break;
    }
}