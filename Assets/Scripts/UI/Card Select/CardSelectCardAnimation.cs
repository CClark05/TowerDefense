using System;
using DG.Tweening;
using UnityEngine;

public class CardSelectCardAnimation : MonoBehaviour
{
    public InterfaceReference<ICardUI> cardUI;
    private Tweener scaleTween;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
        float scaleDuration = 0.25f;
        cardUI.Value.OnHoverCard += () =>
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(originalScale * 1.25f, scaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
        };
        cardUI.Value.OnLeaveHoverCard += () =>
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(originalScale, scaleDuration * 0.75f).SetEase(Ease.OutSine).SetUpdate(true);
        };
    }
}
