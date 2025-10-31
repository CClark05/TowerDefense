using System.Collections;

public class ResetSpeedCommand : EnemyCommand
{
    private float fadeOut;
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().ResetSpeed(fadeOut);
        yield break;
    }
    public ResetSpeedCommand(float fadeOut = 0.25f)
    {
        this.fadeOut = fadeOut;
    }
}