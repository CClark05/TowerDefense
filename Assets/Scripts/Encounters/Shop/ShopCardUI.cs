using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public interface ICardUI
{
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    public event Action OnClickCard;
    public GameObject GameObject { get; }
    public void ToggleButton(bool enabled);
}
public class ShopCardUI : MonoBehaviour, ICardUI
{
    [SerializeField] private TextMeshProUGUI costText;
    private SkillData cardData;
    [SerializeField] private Button_Hover button;
    [SerializeField] private Image overlay;
    [SerializeField] private GameObject soldCardVisualPrefab;
    public static event Action<SkillData> OnBuyCardStatic;
    public static event Action OnNotEnoughCoins;
    public event Action OnBuyCard;
    public event Action OnClickCard;
    public GameObject GameObject => gameObject;
    public void ToggleButton(bool enabled)
    {
        button.enabled = enabled;
    }

    private void Start()
    {
        cardData = GetComponent<SetCardData>().SkillData;
        costText.text = cardData.price.ToString();
        PlayerInventory.Instance.OnCoinsUpdated += coins => SetCostText();
        SetCostText();
        button.OnHover += () => OnHoverCard?.Invoke();
        button.OnLeaveHover += () => OnLeaveHoverCard?.Invoke();
        button.OnClick.AddListener(() =>
        {
            if (PlayerInventory.Instance.Coins < cardData.price)
            {
                OnNotEnoughCoins?.Invoke();
                return;
            }
            if (!InventoryUI.Instance.CanAddCard(cardData))
            {
                Debug.Log("Inventory full");
                return;
            }
            GameObject placeholder = Instantiate(soldCardVisualPrefab, transform.position, Quaternion.identity, transform.parent);
            placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
            placeholder.GetComponentInChildren<CanvasGroup>().DOFade(1, 0.2f).SetEase(Ease.InCubic);
            OnBuyCardStatic?.Invoke(cardData);
            OnBuyCard?.Invoke();
            overlay.color = new Color(0, 0, 0, 0.85f);
            //button.ScaleBackToNormal();
            button.enabled = false;
            costText.transform.parent.gameObject.SetActive(false);
            InventoryUI.Instance.AddCard(cardData.CreateInstance());
            Debug.Log("Bought card: " + cardData.name);
        });
    }

    private void SetCostText() => costText.color = PlayerInventory.Instance.Coins >= cardData.price ? Color.white : ColorPicker.red;
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
   
}
