using System.Collections;

public class ResetSpeedCommand : EnemyCommand
{
    public override IEnumerator Execute(IAgent agent)
    {
        agent.Require<IMovementOverride>().ResetSpeed();
        yield break;
    }
}