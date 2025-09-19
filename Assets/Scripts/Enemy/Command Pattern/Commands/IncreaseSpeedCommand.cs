using System.Collections;
using UnityEngine;

public class IncreaseSpeedCommand : EnemyCommand
{
    private float speedMult;
    public IncreaseSpeedCommand(float duration, float speedMult) : base(duration)
    {
        this.speedMult = speedMult;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().SetSpeed(speedMult);
        yield return new WaitForSeconds(duration);
        agent.Get<IMovementOverride>().ResetSpeed();
    }

    
}