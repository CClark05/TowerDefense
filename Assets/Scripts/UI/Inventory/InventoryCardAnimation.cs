using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryCardAnimation : MonoBehaviour
{
    private SkillCardUI cardUI;
    private CardDragDrop dragDrop;
    private Tweener t;
    private RectTransform rt;
    private float originalY;
    private Vector3 originalScale;
    private Canvas cardCanvas;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Image cardImage;
    private Sprite originalSprite;
    private void Awake()
    {
        cardUI = GetComponent<SkillCardUI>();
        dragDrop = GetComponent<CardDragDrop>();
        rt = GetComponent<RectTransform>();
        cardCanvas = GetComponent<Canvas>();
    }
    Tweener moveTween;
    Tweener scaleTween;
    private void Start()
    {
        originalSprite = cardImage.sprite;
        //cardUI.OnSellCard += RemoveCard;
        originalY = rt.anchoredPosition.y;
        originalScale = transform.localScale;
        GetComponent<CardDragDrop>().OnDropCard += RemoveCard;
        const float moveDuration = 0.25f;
        cardUI.OnHoverCard += () =>
        {
            cardCanvas.sortingOrder = 1;
            moveTween?.Kill();
            scaleTween?.Kill();
            moveTween = rt.DOAnchorPosY(originalY + 50f, moveDuration).SetEase(Ease.OutBack).SetUpdate(true);
            scaleTween = transform.DOScale(originalScale * 1.5f, moveDuration).SetEase(Ease.OutBack).SetUpdate(true);
        };
        cardUI.OnLeaveHoverCard += () =>
        {
            cardCanvas.sortingOrder = 0;
            moveTween?.Kill();
            scaleTween?.Kill();
            moveTween = rt.DOAnchorPosY(originalY, moveDuration * 0.75f).SetEase(Ease.OutSine).SetUpdate(true);
            scaleTween = transform.DOScale(originalScale, moveDuration * 0.75f).SetEase(Ease.OutSine).SetUpdate(true);
        };
        
        cardUI.OnClickedCard += selected =>
        {
            t?.Kill();
            if (selected)
            {
                cardImage.sprite = selectedSprite;
                return;
            }
            cardImage.sprite = originalSprite;

        };
        dragDrop.OnOverTarget += () =>
        {
            GetComponentInChildren<SquishAnimation>().Squish(0.5f);
        };
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack).SetDelay(0.1f).SetUpdate(true);
    }
    
    private void Update()
    {
        if (dragDrop.TargetPosition.HasValue)
        {
            float speed = dragDrop.IsDragging ? 300 : 15;
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, dragDrop.TargetPosition.Value, speed * Time.deltaTime);

            if ((rt.anchoredPosition - dragDrop.TargetPosition.Value).sqrMagnitude <= 0.25f)
            {
                rt.anchoredPosition = dragDrop.TargetPosition.Value;
                dragDrop.ClearTarget();
            }
        }
    }

    private void RemoveCard()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetDelay(0.25f)
            .OnComplete(() =>
            {
                cardUI.RemoveCard(cardUI);
                Destroy(gameObject);
            }).SetUpdate(true);
    }

    public void SlideOverAnimation(Vector3 position, float delay)
    {
        cardUI.ToggleButton(false);
        transform.DOMove(position, 0.23f).SetEase(Ease.OutBack).SetDelay(delay).SetUpdate(true)
            .OnComplete(() => cardUI.ToggleButton(true));
    }
}