using System.Threading;
using DG.Tweening;
using UnityEngine;

public class ShopCardAnimation : MonoBehaviour
{
    private Transform chestLocation;
    [SerializeField] private CanvasGroup canvasGroup;
    public InterfaceReference<ICardUI> cardUI;
    Tweener scaleTween;
    private void Start()
    {
        GetComponent<ShopCardUI>().OnBuyCard += BuyAnimation;
        chestLocation = GetComponentInParent<ShopUI>().ChestLocation;
        Vector3 originalScale = transform.localScale;
        float hoverAnimationDuration = 0.25f;
        
        cardUI.Value.OnHoverCard += () =>
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(transform.localScale * 1.3f, hoverAnimationDuration).SetEase(Ease.OutBack);
        };
        cardUI.Value.OnLeaveHoverCard += () =>
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(originalScale, hoverAnimationDuration * 0.75f).SetEase(Ease.OutSine);
        };
    }

    private void BuyAnimation()
    {
        GetComponent<SetCardData>().DisableTabs();
        Vector3 originalPosition = transform.position;
        Vector3 originalScale = transform.localScale;
        transform.SetParent(transform.parent.parent);
        transform.position = originalPosition;
        transform.localScale = originalScale;
        float moveDuration = 0.75f;
        float flipDuration = 0.085f;
        float liftTime = 0.3f;
        Sequence seq = DOTween.Sequence().SetLink(gameObject);
        seq.Append(transform.DOMoveY(1.25f, liftTime).SetRelative(true)).SetEase(Ease.OutSine);
        seq.Join(transform.DOScale(new Vector3(0.85f, 0.85f, 1), liftTime).SetEase(Ease.OutSine));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOMove(chestLocation.position, moveDuration).SetEase(Ease.InCubic));
        float travelStart = seq.Duration() - moveDuration + 0.2f;
        seq.Insert(travelStart, transform.DOScaleX(0f, flipDuration).SetEase(Ease.OutSine));
        seq.Insert(travelStart + flipDuration, transform.DOScaleX(0.85f, flipDuration).SetEase(Ease.InSine));
        seq.Insert(travelStart, canvasGroup.DOFade(0f, moveDuration).SetEase(Ease.OutCubic)).OnComplete(() => Destroy(gameObject));
    }
}