using CodeMonkey.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIAnimation : MonoBehaviour
{
    private UpgradeUI upgradeUI;
    [SerializeField] private Transform chestLocation;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private Canvas canvas;
    public float AnimationDuration { get; private set; } = 0.4f;
    private void Awake()
    {
        upgradeUI = GetComponent<UpgradeUI>();
        upgradeUI.OnUpgradeCard += (cardUI, slot) =>
        {
            cardUI.GameObject.transform.GetChild(0).DOScale(0.91f, AnimationDuration).SetEase(Ease.OutCubic);
            UITween.MoveToUI(cardUI.GameObject.GetComponent<RectTransform>(), slot.GetComponent<RectTransform>(), canvas, AnimationDuration, Ease.OutCubic, () =>
            {
                Destroy(cardUI.GameObject);
            });
        };
        upgradeUI.OnInputCard += (ICardUI cardUI, Transform slot) =>
        {
            cardUI.GameObject.transform.DOLocalMove(Vector3.zero, AnimationDuration).SetEase(Ease.OutCubic);
            FunctionTimer.Create(() =>
            {
                cardUI.GameObject.GetComponent<SquishAnimation>().Squish(0.3f, 0.6f);
            }, AnimationDuration * 0.75f);

        };
        upgradeUI.OnRemoveInputCard += (cardUI, slot) =>
        {
            UITween.MoveToUI(cardUI.GameObject.GetComponent<RectTransform>(), slot.GetComponent<RectTransform>(), canvas, AnimationDuration, Ease.OutCubic, () =>
            {
                Destroy(cardUI.GameObject);
            });
            cardUI.GameObject.transform.GetChild(0).DOScale(1, AnimationDuration).SetEase(Ease.OutCubic); 
        };
    }
    
}
