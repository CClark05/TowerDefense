using System;
using UnityEngine;
public enum AnimPlayMode
{
    Auto,
    Once,
    Loop,
}


public interface IAnimationPlayer
{
    Coroutine Play(AnimationClip clip, Transform transform, AnimPlayMode mode = AnimPlayMode.Auto,
        float? duration = null, AnimArgs args = null, object owner = null, int layer = 0);

    public Coroutine Play(AnimationClip clip, Transform transform, AnimArgs args,
        AnimPlayMode mode = AnimPlayMode.Auto, float? duration = null, object owner = null, int layer = 0);
    void StopRoutine(object owner, int layer = 0);
    void Cancel(object owner, int layer = 0);
}