using DG.Tweening;
using UnityEngine;

public class CardButton : Button_Hover
{
    [SerializeField] private float scaleFactor = 1.3f;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private Canvas canvas;
    private Vector3 originalScale;
    private int originalSortingOrder;
    private new void OnEnable()
    {
        base.OnEnable();
        originalScale = rectTransform.localScale;
    }
    
    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        DOTween.Kill(gameObject);
        rectTransform.DOScale(rectTransform.localScale * scaleFactor, animationDuration).SetEase(Ease.OutBack).SetTarget(gameObject);
        canvas.overrideSorting = true;
        originalSortingOrder = canvas.sortingOrder;
        canvas.sortingOrder = originalSortingOrder + 1;
    }

    public override void OnMouseLeave()
    {
        base.OnMouseLeave();
        DOTween.Kill(gameObject);
        rectTransform.DOScale(originalScale, animationDuration * 0.75f).SetEase(Ease.OutSine).SetTarget(gameObject);
        canvas.sortingOrder = originalSortingOrder;
        canvas.overrideSorting = false;
    }
}