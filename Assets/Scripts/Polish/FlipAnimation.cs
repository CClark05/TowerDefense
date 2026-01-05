using System;
using DG.Tweening;
using UnityEngine;

public class FlipAnimation : MonoBehaviour
{
    [SerializeField] private Transform toLocation;
    [SerializeField] private float moveDuration = 0.75f;
    [SerializeField] private float flipDuration = 0.085f;
    [SerializeField] private float liftTime = 0.3f;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool playOnAwake;
    [SerializeField] private float delay;
    private void Start()
    {
        if (playOnAwake)
        {
            Flip(toLocation.position);
        }
    }

    public void Flip(Vector3 toPosition)
    {
        GetComponent<SetCardData>().DisableTabs();
        Vector3 originalPosition = transform.position;
        Vector3 originalScale = transform.localScale;
        transform.SetParent(transform.parent.parent);
        transform.position = originalPosition;
        transform.localScale = originalScale;
        Sequence seq = DOTween.Sequence().SetLink(gameObject);
        seq.AppendInterval(delay);
        seq.Append(transform.DOMoveY(1.25f, liftTime).SetRelative(true)).SetEase(Ease.OutSine);
        seq.Join(transform.DOScale(new Vector3(0.85f, 0.85f, 1), liftTime).SetEase(Ease.OutSine));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOMove(toPosition, moveDuration).SetEase(Ease.InCubic));
        float travelStart = seq.Duration() - moveDuration + 0.2f;
        seq.Insert(travelStart, transform.DOScaleX(0f, flipDuration).SetEase(Ease.OutSine));
        seq.Insert(travelStart + flipDuration, transform.DOScaleX(0.85f, flipDuration).SetEase(Ease.InSine));
        seq.Insert(travelStart, canvasGroup.DOFade(0f, moveDuration).SetEase(Ease.OutCubic)).OnComplete(() => Destroy(gameObject));
    }
}
