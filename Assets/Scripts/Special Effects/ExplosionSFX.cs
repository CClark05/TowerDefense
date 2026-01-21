using System;
using UnityEngine;

public class ExplosionSFX : MonoBehaviour
{
    [SerializeField] private AnimationClip explosionAnimation;

    private void Start()
    {
        var animationPlayer = GetComponent<IAnimationPlayer>();
        CameraShake.Shake(Camera.main.transform, 0.5f, 0.6f);
        animationPlayer.Play(explosionAnimation, transform, AnimPlayMode.Once, null, new AnimArgs {fps = 13.3f}).OnComplete += () =>
        {
            Destroy(gameObject);
        };
    }
}
