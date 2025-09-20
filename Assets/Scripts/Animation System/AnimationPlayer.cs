using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour, IAnimationPlayer
{
    private Dictionary<(object owner, int layer), Coroutine> currentAnimations = new();
    public Coroutine Play(AnimationClip clip, Transform transform, AnimPlayMode mode = AnimPlayMode.Auto, float? duration = null, object owner = null, int layer = 0)
    {
        var key = (owner ?? this, layer);
        Stop(owner, layer);
        bool loop = mode switch
        {
            AnimPlayMode.Once => false,
            AnimPlayMode.Loop => true,
            _ => clip.looping
        };
        var routine = StartCoroutine(Run(clip, transform, loop, loop ? clip.defaultDuration : duration ?? clip.defaultDuration, key));
        currentAnimations[key] = routine;
        return routine;
    }

    public void Stop(object owner, int layer = 0)
    {
        var key = (owner ?? this, layer);
        if(currentAnimations.TryGetValue(key, out var routine))
        {
            StopCoroutine(routine);
            currentAnimations.Remove(key);
        }
    }
    
    private IEnumerator Run(AnimationClip clip, Transform transform, bool loop, float duration, (object owner, int layer) key)
    {
        bool Cancelled() => !currentAnimations.ContainsKey(key) || transform == null;
        if (!loop)
        {
            yield return clip.Play(transform, duration, Cancelled);
            currentAnimations.Remove(key);
            yield break;
        }
        while (!Cancelled())
            yield return clip.Play(transform, duration, Cancelled);

        currentAnimations.Remove(key);
    }
}