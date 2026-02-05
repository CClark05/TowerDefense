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
    public static float DropAnimationDuration => 0.75f;
    public static float DropDelayPerCard => 0.1f;
    public event Action OnDoneAnimating;

    private void Awake()
    {
        cardUI.Value.ToggleButton(false);
        UI.SetActive(false);
    }

    public void Init(int cardNumber)
    {
        transform.localPosition = new Vector3(0, 775f, 0);
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
        FunctionTimer.Create(() => UI.SetActive(true), cardNumber * 0.1f);
        GetComponent<RectTransform>().DOAnchorPosY(0, DropAnimationDuration).SetEase(Ease.OutBounce).SetDelay(cardNumber * DropDelayPerCard).OnComplete(() =>
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
