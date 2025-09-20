using System.Collections;
using UnityEngine;

public class StopMovementCommand : EnemyCommand
{
    private float easeTime;
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().SetSpeed(0, easeTime);
        yield break;
    }
    public StopMovementCommand(float easeTime)
    {
        this.easeTime = easeTime;
    }
}