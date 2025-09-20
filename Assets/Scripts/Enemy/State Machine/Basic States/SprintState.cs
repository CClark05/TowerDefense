using UnityEngine;

public class SprintState : BaseState
{
    private float speedMult;
    public SprintState(IAgent agent,float speedMult) : base(agent)
    {
        this.speedMult = speedMult;
    }
    public override void OnEnter()
    {
        Debug.Log("enter sprint");
        runner.Play(this,new ICommand[]
        {
            new IncreaseSpeedCommand(speedMult)
        });
    }

    public override void OnExit()
    {
        Debug.Log("exit sprint");
    }
    
}