using System;
using UnityEngine;

public class StrawberryStateMachine : BossStateMachine
{
    [SerializeField] private AnimationClip digClip, riseClip;
    private void Start()
    {
        var defaultState = new StrawberryDefaultState(facade, 2f);
        var digState = new StrawberryDigState(facade,3, digClip, riseClip);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, digState, new FuncPredicate(() => defaultState.IsDone && digState.IsReady));
        stateMachine.AddTransition(digState, defaultState, new FuncPredicate(() => digState.IsDone));
    }
}

public class StrawberryDigState : BaseState
{
    private AnimationClip digClip;
    private AnimationClip riseClip;
    public StrawberryDigState(IAgent agent, float cooldown, AnimationClip digClip, AnimationClip riseClip) : base(agent, cooldown)
    {
        this.digClip = digClip;
        this.riseClip = riseClip;
    }

    public override void OnEnter()
    {
        var pos = AStarPathfinding.Instance.GetPathPointAhead(agent.Transform.position, 3);
        runner.Play(this, new ICommand[]
        {
            new StopMovementCommand(),
            new PlayAnimationCommand(digClip),
            new TeleportToCommand(pos),
            new PlayAnimationCommand(riseClip),
            new ResetSpeedCommand(),
        });
    }
}
public class StrawberryDefaultState : BaseState
{
    private float duration;
    public StrawberryDefaultState(IAgent agent, float duration, float cooldown = 0) : base(agent, cooldown)
    {
        this.duration = duration;
    }
    public override void OnEnter()
    {
        var randDuration = duration + UnityEngine.Random.Range(0, 2f);
        runner.Play(this, new ICommand[]
        {
            new WaitCommand(randDuration),
        });
    }
}