using System;
using CodeMonkey.Utils;
using DG.Tweening;
using UnityEngine;

public class CardSelectCardAnimation : MonoBehaviour
{
    public InterfaceReference<ICardUI> cardUI;
    [SerializeField] private GameObject UI;
    private Tweener scaleTween;
    private Vector3 originalScale;
    public static float AnimationDuration => 0.25f;
    public static float DelayPerCard => 0.075f;
    public event Action OnDoneAnimating;

    private void Awake()
    {
        cardUI.Value.ToggleButton(false);
    }

    public void Init(int cardNumber)
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

        transform.localScale = Vector3.zero;
        GetComponent<RectTransform>().DOScale(1, AnimationDuration).SetEase(Ease.OutCubic).SetDelay(cardNumber * DelayPerCard).OnComplete(() =>
        {
            cardUI.Value.ToggleButton(true);
            OnDoneAnimating?.Invoke();
        });
    }
    public void AnimateBack()
    {
        GetComponent<RectTransform>().DOAnchorPosY(775f, 0.5f).SetEase(Ease.InBack);
    }

    public void MoveToInventory(Transform cardSlot, Action OnComplete)
    {
        float duration = 0.35f;
        Sequence seq = DOTween.Sequence();
        seq.Append(UI.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InCubic));
        seq.Join(GetComponent<RectTransform>().DOMove(cardSlot.position, duration).SetEase(Ease.InCubic));
        seq.AppendCallback(() =>
        {
            OnComplete?.Invoke();
        }).SetDelay(0.1f);
    }
}
