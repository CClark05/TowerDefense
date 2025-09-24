using System;
using System.Collections;
using UnityEngine;

public abstract class AnimationClip : ScriptableObject
{
    public abstract IEnumerator Play(Transform transform, float duration, Func<bool> cancelled, AnimArgs args = null);
    [HideInInspector] public float defaultDuration = 0.5f;
    public bool looping;
}

public class AnimArgs
{
    public Vector2? scaleTo;
}