using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InventoryCardAnimation : MonoBehaviour
{
    private SkillCardUI cardUI;
    private CardDragDrop dragDrop;
    private Tweener t;
    private RectTransform rt;
    public Vector3 originalPos;
    private Vector3 originalScale;
    private Canvas cardCanvas;
    [SerializeField] private Image cardImage, icon;
    private Sprite originalSprite;
    [SerializeField] private GameObject UI;
    [SerializeField] private Sprite cardBackSprite;
    private void Awake()
    {
        cardUI = GetComponent<SkillCardUI>();
        dragDrop = GetComponent<CardDragDrop>();
        rt = GetComponent<RectTransform>();
        cardCanvas = GetComponent<Canvas>();
    }
    Tweener moveTween;
    Tweener scaleTween;
    public bool Dropped;
    private void Start()
    {
        originalSprite = cardImage.sprite;
        //cardUI.OnSellCard += RemoveCard;
        originalPos = rt.anchoredPosition;
        originalScale = transform.localScale;
        GetComponent<CardDragDrop>().OnDropCard += RemoveCard;
        const float moveDuration = 0.25f;
        cardUI.OnHoverCard += () =>
        {
            if (dragDrop.TargetPosition.HasValue) return;
            GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
            originalPos = rt.anchoredPosition;
            cardCanvas.sortingOrder = 1;
            moveTween?.Kill();
            scaleTween?.Kill();
            moveTween = rt.DOAnchorPosY(originalPos.y + 50f, moveDuration).SetEase(Ease.OutBack).SetUpdate(true);
            scaleTween = transform.DOScale(originalScale * 1.5f, moveDuration).SetEase(Ease.OutBack).SetUpdate(true);
        };
        cardUI.OnLeaveHoverCard += () =>
        {
            if (Dropped) return;
            GameManager.Instance.SetCursor(GameManager.Cursors.Default);
            cardCanvas.sortingOrder = 0;
            moveTween?.Kill();
            scaleTween?.Kill();
            moveTween = rt.DOAnchorPosY(originalPos.y, moveDuration * 0.75f).SetEase(Ease.OutSine).SetUpdate(true);
            scaleTween = transform.DOScale(originalScale, moveDuration * 0.75f).SetEase(Ease.OutSine).SetUpdate(true);
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
        Dropped = true;
        enabled = false;
        UI.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetDelay(0.25f)
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
            .OnComplete(() =>
            {
                cardUI.ToggleButton(true);
                slotPosition = transform.position;
            });
        
    }

    private Vector3 slotPosition;
    public void AnimateIntoHand(Transform handTransform, float delay)
    {
        float duration = 0.28f;
        cardUI.ToggleButton(false);
        transform.DOKill();
        slotPosition = transform.position;
        Vector3 start = transform.position;
        Vector3 end = handTransform.position;
        Vector3 control = (start + end) * 0.5f + Vector3.up * 1.2f;

        var seq = DOTween.Sequence().SetDelay(delay).SetUpdate(true);

        seq.Append(transform.DOScale(transform.localScale * 1.04f, 0.06f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            icon.gameObject.SetActive(false);
            cardImage.sprite = cardBackSprite;
            foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
                text.gameObject.SetActive(false);
        }));
        seq.Append(transform.DOPath(new[] { start, control, end }, duration, PathType.CatmullRom).SetEase(Ease.OutCubic));
        seq.Join(transform.DOScale(Vector3.one * 0.5f, duration).SetEase(Ease.OutCubic));
        seq.Append(transform.DOScale(Vector3.one * 0.5f * 1.03f, 0.05f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(Vector3.one * 0.5f, 0.07f).SetEase(Ease.InOutSine));

        seq.OnComplete(() => gameObject.SetActive(false));
    }
    public void AnimateBack(float delay = 0f)
    {
        float duration = 0.28f;

        transform.DOKill();
        gameObject.SetActive(true);

        Vector3 start = transform.position;
        Vector3 end = slotPosition;
        Vector3 control = (start + end) * 0.5f + Vector3.up * 1.2f;

        var seq = DOTween.Sequence().SetDelay(delay).SetUpdate(true);

        seq.Append(transform.DOPath(
            new[] { start, control, end },
            duration,
            PathType.CatmullRom
        ).SetEase(Ease.OutCubic));

        seq.Join(transform.DOScale(originalScale * 1.04f, duration * 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            cardUI.ToggleButton(true);
            icon.gameObject.SetActive(true);
            cardImage.sprite = originalSprite;
            foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
                text.gameObject.SetActive(true);
        }));
        seq.Append(transform.DOScale(originalScale, duration * 0.4f).SetEase(Ease.InOutSine));
    }


}