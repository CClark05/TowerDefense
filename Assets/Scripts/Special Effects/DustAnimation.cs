using System;
using UnityEngine;

public class DustAnimation : MonoBehaviour
{
    [SerializeField] private AnimationClip animation;

    private void Start()
    {
        var anim = GetComponent<IAnimationPlayer>().Play(animation, transform);
        anim.OnComplete += () =>
        {
            Destroy(gameObject);
        };
    }
}
