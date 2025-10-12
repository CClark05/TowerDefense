using System;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerStateMachine : BossStateMachine
{
    [SerializeField] private AnimationClip summonAnimation, moveAnimation;
    private void Start()
    {
        var defaultState = new NecromancerDefaultState(facade, 3, moveAnimation);
        var summonState = new NecromancerSummonState(facade, 3f, defaultState.DeadEnemies, 6f, summonAnimation);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, summonState, new FuncPredicate(() => defaultState.IsDone && summonState.IsReady));
        stateMachine.AddTransition(summonState, defaultState, new FuncPredicate(() => summonState.IsDone));
    }
}
public class NecromancerSummonState : BaseState
{
    private float summonDuration;
    private List<EnemyData> deadEnemies;
    private AnimationClip summonAnimation;
    public NecromancerSummonState(IAgent agent, float summonDuration, List<EnemyData> deadEnemies, float cooldown, AnimationClip summonAnimation) : base(agent, cooldown)
    {
        this.summonDuration = summonDuration;
        this.deadEnemies = deadEnemies;
        this.summonAnimation = summonAnimation;
    }
    public override void OnEnter()
    {
        Debug.Log("Enter summon");
        agent.Require<IAnimationPlayer>().Play(summonAnimation, agent.Transform);
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
    private AnimationClip idleClip;
    public NecromancerDefaultState(IAgent agent, int minEnemiesKilled, AnimationClip idleClip) : base(agent)
    {
        this.minEnemiesKilled = minEnemiesKilled;
        this.idleClip = idleClip;
    }
    public override void OnEnter()
    {
        Debug.Log("Enter default");
        agent.Require<IAnimationPlayer>().Play(idleClip, agent.Transform);
        runner.Play(this, new ICommand[]
        {
            new WaitForDeadEnemiesCommand(minEnemiesKilled, DeadEnemies),
        });
    }
    
    
}