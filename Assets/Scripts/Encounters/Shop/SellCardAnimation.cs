using System;
using DG.Tweening;
using UnityEngine;

public class SellCardAnimation : MonoBehaviour
{
    private SellCardUI sellCardUI;
    private Tweener scaleTween;
    [SerializeField] private CanvasGroup canvasGroup;
    private Canvas canvas;
    private int originalSortingOrder;
    private void Awake()
    {
        sellCardUI = GetComponent<SellCardUI>();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        sellCardUI.OnSellCard += SellAnimation;
        float hoverAnimationDuration = 0.25f;
        Vector3 originalScale = transform.localScale;
        sellCardUI.OnHoverCard += () =>
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(transform.localScale * 1.3f, hoverAnimationDuration).SetEase(Ease.OutBack);
            canvas.overrideSorting = true;
            originalSortingOrder = canvas.sortingOrder;
            canvas.sortingOrder = originalSortingOrder + 1;
        };
        sellCardUI.OnLeaveHoverCard += () =>
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(originalScale, hoverAnimationDuration * 0.75f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                canvas.sortingOrder = originalSortingOrder;
                canvas.overrideSorting = false;
            });
            
        };
    }

    private void SellAnimation()
    {
        Vector3 originalPosition = transform.position;
        Vector3 originalScale = transform.localScale;
        transform.SetParent(transform.parent.parent);
        transform.position = originalPosition;
        transform.localScale = originalScale;
        Sequence seq = DOTween.Sequence().SetLink(gameObject);
        float liftTime = 0.3f;
        seq.Append(transform.DOMoveY(1.25f, liftTime).SetRelative(true)).SetEase(Ease.OutSine);
        seq.Join(transform.DOScale(new Vector3(0.85f, 0.85f, 1), liftTime).SetEase(Ease.OutSine));
        seq.AppendInterval(0.5f);
        seq.Append(canvasGroup.DOFade(0, 0.25f)).OnComplete(() => Destroy(gameObject));
    }
}