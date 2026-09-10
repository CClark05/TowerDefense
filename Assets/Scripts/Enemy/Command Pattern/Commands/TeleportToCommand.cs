using System.Collections;
using UnityEngine;

public class TeleportToCommand : EnemyCommand
{
    Vector2 targetPosition;
    public TeleportToCommand(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Transform.GetComponent<IMovementOverride>().TeleportTo(targetPosition);
        yield break;
    }
}