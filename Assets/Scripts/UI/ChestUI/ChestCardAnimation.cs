using System;
using DG.Tweening;
using UnityEngine;

public class ChestCardAnimation : MonoBehaviour
{
    public InterfaceReference<ICardUI> cardUI;
    [SerializeField] private GameObject UI;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void MoveToInventory(Transform cardSlot, Action OnComplete)
    {
        float duration = 0.35f;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InCubic));
        seq.Join(UI.transform.DOMove(cardSlot.position, duration).SetEase(Ease.InCubic));
        seq.AppendCallback(() =>
        {
            OnComplete?.Invoke();
        }).SetDelay(0.1f);
    }

    public void MoveTo(Vector3 position, Action OnComplete)
    {
        float duration = 0.35f;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(Vector3.one, duration).From(Vector3.zero).SetEase(Ease.OutCubic));
        seq.Join(UI.transform.DOMove(position, duration).SetEase(Ease.OutCubic));
        seq.AppendCallback(() =>
        {
            OnComplete?.Invoke();
        });
    }
}
