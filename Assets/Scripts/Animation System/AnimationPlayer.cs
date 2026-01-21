using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public sealed class AnimationHandle
{
    public event Action OnComplete;
    internal void Complete() => OnComplete?.Invoke();
}
public class AnimationPlayer : MonoBehaviour, IAnimationPlayer
{
    
    private Dictionary<(object owner, int layer), Coroutine> currentAnimations = new();
    public AnimationHandle Play(AnimationClip clip, Transform transform, AnimPlayMode mode = AnimPlayMode.Auto, float? duration = null, AnimArgs args = null, object owner = null, int layer = 0)
    {
        var key = (owner ?? this, layer);
        StopRoutine(owner, layer);
        currentAnimations[key] = null;
        bool loop = mode switch
        {
            AnimPlayMode.Once => false,
            AnimPlayMode.Loop => true,
            _ => clip.looping
        };
        var handle = new AnimationHandle();
        var routine = StartCoroutine(Run(clip, transform, loop, loop ? clip.defaultDuration : duration ?? clip.defaultDuration, key, args, handle));
        currentAnimations[key] = routine;
        return handle;
    }

    public AnimationHandle Play(AnimationClip clip, Transform transform, AnimArgs args, AnimPlayMode mode = AnimPlayMode.Auto, float? duration = null, object owner = null, int layer = 0)
    {
        return Play(clip, transform, mode, duration, args, owner, layer);
    }
        
    public void StopRoutine(object owner, int layer = 0)
    {
        var key = (owner ?? this, layer);
        if (currentAnimations.TryGetValue(key, out var routine))
        {
            StopCoroutine(routine);
            currentAnimations.Remove(key);
        }
    }

    public void Cancel(object owner, int layer = 0)
    {
        var key = (owner ?? this, layer);
        currentAnimations.Remove(key);
    }

    private IEnumerator Run(
        AnimationClip clip,
        Transform transform,
        bool loop,
        float duration,
        (object owner, int layer) key,
        AnimArgs args,
        AnimationHandle handle)
    {
        bool Cancelled() =>
            !currentAnimations.ContainsKey(key) || transform == null;

        if (!loop)
        {
            yield return clip.Play(transform, duration, Cancelled, args);

            if (!Cancelled())
            {
                if (clip is SpriteAnimationClip animationClip)
                    animationClip.ApplyLastFrame(transform, args);
                handle.Complete();
            }
             

            currentAnimations.Remove(key);
            yield break;
        }

        while (!Cancelled())
            yield return clip.Play(transform, duration, Cancelled, args);

        currentAnimations.Remove(key);
    }
}