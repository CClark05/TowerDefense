using System;
using DG.Tweening;
using UnityEngine;

public class CardButton : Button_Hover
{
    [SerializeField] private float scaleFactor = 1.3f;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private Canvas canvas;
    private Vector3? originalScale;
    private int originalSortingOrder;
    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (originalScale == null)
            originalScale = rectTransform.localScale;
        
        DOTween.Kill(gameObject);
        rectTransform.DOScale(originalScale.Value * scaleFactor, animationDuration).SetEase(Ease.OutBack).SetTarget(gameObject);
        if (canvas == null) return;
        canvas.overrideSorting = true;
        originalSortingOrder = canvas.sortingOrder;
        canvas.sortingOrder = originalSortingOrder + 1;
    }

    public override void OnMouseLeave()
    {
        base.OnMouseLeave();
        DOTween.Kill(gameObject);
        rectTransform.DOScale(originalScale.Value, animationDuration * 0.75f).SetEase(Ease.OutSine).SetTarget(gameObject);
        if (canvas == null) return;
        canvas.sortingOrder = originalSortingOrder;
        canvas.overrideSorting = false;
    }
}