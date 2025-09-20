using System.Collections;
using System.Collections.Generic;

public class GetTowersCommand : EnemyCommand
{
    private float radius;
    private List<TowerDataHolder> towers;
    public GetTowersCommand(float radius, List<TowerDataHolder> towers)
    {
        this.radius = radius;
        this.towers = towers;
    }

    public override IEnumerator Execute(IAgent agent)
    {
        towers.Clear();
        towers.AddRange(agent.Require<IGetNearbyTowers>().GetNearbyTowers(radius));
        yield return null;
    }
}