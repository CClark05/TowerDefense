using System.Collections;
using UnityEngine;

public class DefaultCommand : EnemyCommand
{
    public override IEnumerator Execute(IAgent agent)
    {
        yield return new WaitForSeconds(duration);
    }

    public DefaultCommand(float duration) : base(duration)
    {
    }
}