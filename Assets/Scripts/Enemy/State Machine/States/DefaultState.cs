using UnityEngine;

public class DefaultState : BaseState
{
    public DefaultState(IAgent agent, float duration) : base(agent, duration)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("enter default");
        runner.Play(this,new ICommand[]
        {
            new DefaultCommand(duration)
        });
    }
}