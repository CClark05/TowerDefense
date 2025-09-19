using UnityEngine;

public class SprintState : BaseState
{
    private float speedMult;
    public SprintState(IAgent agent, float duration, float speedMult) : base(agent, duration)
    {
        this.speedMult = speedMult;
    }
    public override void OnEnter()
    {
        Debug.Log("enter sprint");
        runner.Play(this,new ICommand[]
        {
            new IncreaseSpeedCommand(duration, speedMult)
        });
    }

    public override void OnExit()
    {
        Debug.Log("exit sprint");
    }


    
}