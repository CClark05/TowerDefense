using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(RectTransform))]
public class UIScaleLoop : MonoBehaviour
{
    [SerializeField] private float scaleUpFactor = 1.2f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private int loops = -1; // -1 = infinite
    [SerializeField] private bool playOnAwake;
    [SerializeField] private Ease ease = Ease.InOutSine;

    private Tween scaleTween;
    private Vector3 originalScale;
    private RectTransform rect;

    private const float EPS = 0.0001f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;
    }

    private void Start()
    {
        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        // if a loop is already running, don't start another
        if (scaleTween != null && scaleTween.IsActive() && scaleTween.IsPlaying())
            return;

        // kill any stale tween (paused/completed but still referenced)
        scaleTween?.Kill();
        scaleTween = null;

        // base "up" distance used to compute a consistent speed
        Vector3 upTarget = originalScale * scaleUpFactor;
        float baseDistance = Vector3.Distance(originalScale, upTarget);

        // if scaleUpFactor is ~1 (no yoyo movement), just snap to original and bail
        if (baseDistance < EPS)
        {
            rect.localScale = originalScale;
            return;
        }

        // if we're off the original scale, glide back first at matching speed profile
        float distBack = Vector3.Distance(rect.localScale, originalScale);
        if (distBack > EPS)
        {
            float backDuration = duration * (distBack / baseDistance);

            // animate back to original, then start the loop
            rect.DOScale(originalScale, backDuration)
                .SetEase(ease)
                .OnComplete(StartLoop);
        }
        else
        {
            // already at original, start looping immediately
            StartLoop();
        }
    }

    private void StartLoop()
    {
        // start the yoyo loop from original -> upTarget
        scaleTween = rect.DOScale(originalScale * scaleUpFactor, duration)
            .SetEase(ease)
            .SetLoops(loops, LoopType.Yoyo);
    }

    public void Stop(bool resetToOriginal = false)
    {
        scaleTween?.Kill();
        scaleTween = null;

        if (resetToOriginal)
            rect.localScale = originalScale;
    }
}