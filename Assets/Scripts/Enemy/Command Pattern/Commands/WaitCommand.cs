using System.Collections;
using UnityEngine;

public class WaitCommand : EnemyCommand
{
    private float duration;
    public WaitCommand(float duration)
    {
        this.duration = duration;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        yield return new WaitForSeconds(duration);
    }
}