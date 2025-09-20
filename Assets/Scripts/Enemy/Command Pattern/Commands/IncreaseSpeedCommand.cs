using System.Collections;
using UnityEngine;

public class IncreaseSpeedCommand : EnemyCommand
{
    private float speedMult;
    public IncreaseSpeedCommand(float speedMult)
    {
        this.speedMult = speedMult;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().SetSpeed(speedMult);
        yield break;
    }
}