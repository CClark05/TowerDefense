using System;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerStateMachine : BossStateMachine
{
    private void Start()
    {
        var defaultState = new NecromancerDefaultState(facade, 3);
        var summonState = new NecromancerSummonState(facade, 3f, defaultState.DeadEnemies, 4f);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, summonState, new FuncPredicate(() => defaultState.IsDone && summonState.IsReady));
        stateMachine.AddTransition(summonState, defaultState, new FuncPredicate(() => summonState.IsDone));
    }
}
public class NecromancerSummonState : BaseState
{
    private float summonDuration;
    private List<EnemyData> deadEnemies;
    public NecromancerSummonState(IAgent agent, float summonDuration, List<EnemyData> deadEnemies, float cooldown) : base(agent, cooldown)
    {
        this.summonDuration = summonDuration;
        this.deadEnemies = deadEnemies;
    }
    public override void OnEnter()
    {
        Debug.Log("Enter summon");
        runner.Play(this, new ICommand[]
        {
            new StopMovementCommand(0.25f),
            new WaitCommand(0.5f),
            new SpawnEnemyCommand(deadEnemies, agent.Transform.position),
            new WaitCommand(summonDuration),
            new ResetSpeedCommand(0.25f),
        });
    }
}

public class NecromancerDefaultState : BaseState
{
    private int minEnemiesKilled;
    public List<EnemyData> DeadEnemies { get; private set; } = new();
    public NecromancerDefaultState(IAgent agent, int minEnemiesKilled) : base(agent)
    {
        this.minEnemiesKilled = minEnemiesKilled;
    }
    public override void OnEnter()
    {
        Debug.Log("Enter default");
        runner.Play(this, new ICommand[]
        {
            new WaitForDeadEnemiesCommand(minEnemiesKilled, DeadEnemies),
        });
    }
    
}