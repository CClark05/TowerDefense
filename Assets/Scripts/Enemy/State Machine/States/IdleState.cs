using UnityEngine;

public class IdleState : BaseState
{
    private float slowTime;
    public override void OnEnter()
    {
        Debug.Log("enter idle");
        runner.Play(this,new ICommand[]
        {
            new IdleCommand(duration, slowTime)
        });
    }

    public override void OnExit()
    {
        Debug.Log("exit idle");
    }

    public IdleState(IAgent agent, float duration, float slowTime) : base(agent, duration)
    {
        this.slowTime = slowTime;
    }
}