using System.Collections;
using UnityEngine;

public class PlayAnimationCommand : EnemyCommand
{
    private AnimationClip clip;
    public PlayAnimationCommand(AnimationClip clip)
    {
        this.clip = clip;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        var handle = agent.Require<IAnimationPlayer>().Play(clip, agent.Transform);
        bool isDone = false;
        handle.OnComplete += () => isDone = true;
        yield return new WaitUntil(() => isDone);
    }
}