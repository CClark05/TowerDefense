using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpCommand : EnemyCommand
{
    private float jumpHeight;
    private float duration;

    public JumpCommand(float duration, float jumpHeight)
    {
        this.duration = duration;
        this.jumpHeight = jumpHeight;
    }

    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().SetSpeed(0);
        agent.Get<IMovementOverride>().Jump(jumpHeight, duration);
        yield return new WaitForSeconds(duration);
        agent.Get<IMovementOverride>().ResetSpeed();
    }
}