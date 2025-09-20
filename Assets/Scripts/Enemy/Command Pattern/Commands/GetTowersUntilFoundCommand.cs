using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTowersUntilFoundCommand : EnemyCommand
{
    private float radius;
    private List<TowerDataHolder> towers;
    private float pollingInterval;
    public GetTowersUntilFoundCommand(float radius, List<TowerDataHolder> towers, float pollingInterval = 0.1f)
    {
        this.radius = radius;
        this.towers = towers;
        this.pollingInterval = pollingInterval;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        var nearbyTowers = agent.Require<IGetNearbyTowers>();
        while (true)
        {
            towers.Clear();
            towers.AddRange(nearbyTowers.GetNearbyTowers(radius));
            if (towers.Count > 0)
                yield break;

            yield return new WaitForSeconds(pollingInterval);
        }
    }
}
