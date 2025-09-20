using System.Collections.Generic;
using UnityEngine;

public class SlimeDefaultState : BaseState
{
    private List<TowerDataHolder> nearbyTowers = new();
    private float detectionRadius;
    public SlimeDefaultState(IAgent agent, float detectionRadius) : base(agent)
    {
        this.detectionRadius = detectionRadius;
    }

    public override void OnEnter()
    {
        Debug.Log("Enter default");
        runner.Play(this, new ICommand[]
        {
            new GetTowersUntilFoundCommand(detectionRadius, nearbyTowers),
        });
    }
}
