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

    private void Start()
    {
        foreach(var button in towerButtons)
        {
            button.OnClick += () => BuyTower(button.Data);
        }
    }

    private void BuyTower(TowerData data)
    {
        canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
        {
            canvasGroup.gameObject.SetActive(false);
        });
        BuildingManager.Instance.EnterBuildMode(data);
        
    }
}
