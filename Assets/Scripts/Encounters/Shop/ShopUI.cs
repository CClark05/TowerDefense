using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TowerShopButton[] towerButtons;
    [SerializeField] private SOEvent OnShowShop;
    [SerializeField] private Button_Base leaveButton;
    private float fadeDuration = 0.2f;
    private void Start()
    {
        foreach(var button in towerButtons)
        {
            button.OnClick += () => BuyTower(button.Data);
        }
        OnShowShop.OnRaised += (sender) =>
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1, fadeDuration);
        };
        leaveButton.OnClick.AddListener(() =>
        {
            canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
            {
                canvasGroup.gameObject.SetActive(false);
            });
        });
    }

    private void BuyTower(TowerData data)
    {
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
        });
        BuildingManager.Instance.EnterBuildMode(data);
        
    }
}
