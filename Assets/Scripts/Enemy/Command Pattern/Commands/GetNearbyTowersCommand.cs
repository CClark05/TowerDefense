using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetNearbyTowersCommand : EnemyCommand
{
    private float radius;
    private List<TowerDataHolder> towers;
    private int count;
    public GetNearbyTowersCommand(List<TowerDataHolder> towers, int? count = null, float? radius = null)
    {
        this.radius = radius ?? float.MaxValue;
        this.towers = towers;
        this.count = count ?? int.MaxValue;
    }

    public override IEnumerator Execute(IAgent agent)
    {
        towers.Clear();
        var nearbyTowers = agent.Require<IGetNearbyTowers>().GetNearbyTowers(radius);
        for (int i = 0; i < Mathf.Min(count, nearbyTowers.Length); i++)
        {
            towers.Add(nearbyTowers[i]);
        }
        yield break;
    }
}