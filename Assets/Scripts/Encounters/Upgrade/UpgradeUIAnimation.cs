using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIAnimation : MonoBehaviour
{
    private UpgradeUI upgradeUI;
    [SerializeField] private Transform chestLocation;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private Canvas canvas;
    public float AnimationDuration { get; private set; } = 0.5f;
    private void Awake()
    {
        upgradeUI = GetComponent<UpgradeUI>();
        upgradeUI.OnUpgradeCard += (cardUI, slot) =>
        {
            cardUI.GameObject.transform.GetChild(0).DOScale(0.68f, AnimationDuration).SetEase(Ease.OutCubic);
            UITween.MoveToUI(cardUI.GameObject.GetComponent<RectTransform>(), slot.GetComponent<RectTransform>(), canvas, AnimationDuration, Ease.OutCubic, () =>
            {
                Destroy(cardUI.GameObject);
            });
        };
        upgradeUI.OnInputCard += (ICardUI cardUI, Transform slot) =>
        {
            cardUI.GameObject.transform.DOLocalMove(Vector3.zero, AnimationDuration).SetEase(Ease.OutCubic);
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
