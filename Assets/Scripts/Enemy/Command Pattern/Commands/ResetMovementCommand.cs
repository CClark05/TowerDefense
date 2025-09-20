using System.Collections;

public class ResetMovementCommand : EnemyCommand
{
    private float fadeOut;
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().ResetSpeed(fadeOut);
        yield break;
    }
    public ResetMovementCommand(float fadeOut)
    {
        this.fadeOut = fadeOut;
    }
}