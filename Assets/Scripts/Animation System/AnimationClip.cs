using System;
using System.Collections;
using UnityEngine;

public abstract class AnimationClip : ScriptableObject
{
    public abstract IEnumerator Play(Transform transform, float duration, Func<bool> cancelled);
    [HideInInspector] public float defaultDuration = 0.5f;
    public bool looping;
}