using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerShopButton : MonoBehaviour
{
    [SerializeField] private TowerData data;
    public TowerData Data => data;
    [SerializeField] private TextMeshProUGUI nameText, priceText;
    [SerializeField] private Button_Hover button;
    [SerializeField] private Image overlay;
    public event Action OnHover;
    public event Action OnLeaveHover;
    public event Action OnClick;
    private void Start()
    {
        nameText.text = data.objectName;
        priceText.text = data.cost.ToString();
        PlayerInventory.Instance.OnCoinsUpdated += CoinsUpdated;
        CoinsUpdated(PlayerInventory.Instance.Coins);
        button.OnHover += () => OnHover?.Invoke();
        button.OnLeaveHover += () => OnLeaveHover?.Invoke();
        button.OnClick.AddListener(() =>
        {
            OnClick?.Invoke();
        });
    }

    private void CoinsUpdated(int coins)
    {
        priceText.color = coins >= data.cost ? Color.white : ColorPicker.red;
        if (coins < data.cost)
        {
            button.enabled = false;
            overlay.DOFade(0.5f, 0.2f);
            return;
        }
        button.enabled = true;
        overlay.DOFade(0f, 0.2f);
    }
}

