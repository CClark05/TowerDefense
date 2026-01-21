using System;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerStateMachine : BossStateMachine
{
    [SerializeField] private AnimationClip summonAnimation, moveAnimation;
    [SerializeField] private BuffData stunDebuff;
    private int maxSummons = 3;
    public int summonCounter;
    private void Start()
    {
        var healthPredicate = new HealthPercentagePredicate(facade.GetComponent<IUsesHealth>());
        var defaultState = new NecromancerDefaultState(facade, 2, moveAnimation);
        var summonState = new NecromancerSummonState(facade, 1f, 4, 5f, summonAnimation, 3);
        var attackState = new NecromancerAttackState(facade, 3, 3, 3, stunDebuff);
        summonState.OnEnterState += () => summonCounter++;
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, summonState, new FuncPredicate(() => defaultState.IsDone && summonState.IsReady && summonCounter < maxSummons));
        stateMachine.AddTransition(summonState, defaultState, new FuncPredicate(() => summonState.IsDone));
        stateMachine.AddTransition(defaultState, attackState, new CompositePredicate(new FuncPredicate(() => defaultState.IsDone && attackState.IsReady), healthPredicate));
        stateMachine.AddTransition(attackState, defaultState, new FuncPredicate(() => attackState.IsDone));
    }
}

public class NecromancerAttackState : BaseState
{
    List<TowerDataHolder> nearbyTowers = new();
    private BuffData stunDebuff;
    private int stunStacks;
    private int shields;

    public NecromancerAttackState(IAgent agent, float cooldown, int stunStacks, int shields, BuffData stunDebuff) : base(agent, cooldown)
    {
        this.stunDebuff = stunDebuff;
        this.stunStacks = stunStacks;
        this.shields = shields;
    }

    public override void OnEnter()
    {
        Debug.Log("Necromancer attack state entered");
        runner.Play(this, new ICommand[]
        {
            new GainShields(shields),
            new StopMovementCommand(0.1f),
            new GetNearbyTowersCommand(nearbyTowers, 1),
            new ApplyBuffToTowersCommand(stunDebuff, stunStacks, nearbyTowers),
            new ResetSpeedCommand(0)
        });
    }
}

public class NecromancerSummonState : BaseState
{
    private int summonCount;
    private float summonDuration;
    private AnimationClip summonAnimation;
    private int shields;
    private List<EnemyData> summons = new();
    public event Action OnEnterState;
    public NecromancerSummonState(IAgent agent, float summonDuration, int summonCount, float cooldown, AnimationClip summonAnimation, int shields) : base(agent, cooldown)
    {
        this.summonDuration = summonDuration;
        this.summonCount = summonCount;
        this.summonAnimation = summonAnimation;
        this.shields = shields;
    }

    public override void OnEnter()
    {
        Debug.Log("Necromancer summon state entered");
        OnEnterState?.Invoke();
        agent.Require<IAnimationPlayer>().Play(summonAnimation, agent.Transform);
        runner.Play(this, new ICommand[]
        {
            new GainShields(shields),
            new StopMovementCommand(),
            new WaitCommand(0.5f),
            new GetDeadEnemiesCommand(summons, summonCount),
            new SpawnEnemiesCommand(summons, agent.Transform.position, 1f),
            new WaitCommand(summonDuration),
            new ResetSpeedCommand(),
        });
    }
}

public class NecromancerDefaultState : BaseState
{
    private float duration;
    private AnimationClip idleClip;

    public NecromancerDefaultState(IAgent agent, float duration, AnimationClip idleClip) : base(agent)
    {
        this.duration = duration;
        this.idleClip = idleClip;
    }

    public override void OnEnter()
    {
        Debug.Log("Necromancer default state entered");
        agent.Require<IAnimationPlayer>().Play(idleClip, agent.Transform);
        runner.Play(this, new ICommand[]
        {
            new WaitCommand(duration),
        });
    }
}