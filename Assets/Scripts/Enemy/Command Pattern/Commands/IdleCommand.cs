using System.Collections;
using UnityEngine;

public class IdleCommand : EnemyCommand
{
    private float slowTime;
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().SetSpeed(0, slowTime);
        yield return new WaitForSeconds(duration);
        agent.Get<IMovementOverride>().ResetSpeed(slowTime);
    }

    public IdleCommand(float duration, float slowTime) : base(duration)
    {
        this.slowTime = slowTime;
    }
}