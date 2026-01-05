using System;
using DG.Tweening;
using UnityEngine;

public static class UITween
{
    
    public static Tween MoveToUI(
        RectTransform mover,
        RectTransform target,
        Canvas canvas,
        float duration,
        Ease ease = Ease.OutCubic,
        Action onComplete = null)
    {
        var parent = (RectTransform)mover.parent;
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        mover.DOKill();

        Vector2 start = mover.anchoredPosition;
        float t = 0f;

        Vector2 GetDest()
        {
            Vector3 targetWorld = target.TransformPoint(target.rect.center);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, targetWorld);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen, cam, out var dest);
            return dest;
        }

        return DOTween.To(() => t, x =>
                {
                    t = x;
                    mover.anchoredPosition = Vector2.LerpUnclamped(start, GetDest(), t);
                },
                1f, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }
}