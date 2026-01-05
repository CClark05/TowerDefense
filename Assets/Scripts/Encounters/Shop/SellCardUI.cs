using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SellCardUI : MonoBehaviour, ICardUI
{
    [SerializeField] private Button_Hover button;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject soldCardPlaceholder;
    [SerializeField] private CanvasGroup canvasGroup;
    private float sellRate = 0.5f;
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    public event Action OnClickCard;
    public GameObject GameObject => gameObject;
    private int sellPrice;
    public event Action OnSellCard;
    private void Start()
    {
        sellPrice = Mathf.RoundToInt(GetComponent<SetCardData>().SkillData.price * sellRate);
        costText.text = sellPrice.ToString();
        button.OnHover += () => OnHoverCard?.Invoke();
        button.OnLeaveHover += () => OnLeaveHoverCard?.Invoke();
        button.OnClick.AddListener(() =>
        {
            GameObject placeholder = Instantiate(soldCardPlaceholder, transform.position, Quaternion.identity, transform.parent);
            placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());
            OnSellCard?.Invoke();
            costText.gameObject.SetActive(false);
            costText.transform.parent.gameObject.SetActive(false);
        });
    }

    public void ToggleButton(bool enable)
    {
        button.enabled = enable;
        canvasGroup.alpha = enable ? 1 : 0.5f;
    }
}
