using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectUIOLD : MonoBehaviour
{
    /**
    [SerializeField] private TextMeshProUGUI nameText, levelText, damageText, fireRateText, DPSText, enemiesKilledText, totalDamageText, sellPriceText;
    [SerializeField] private GameObject UI;
    //[SerializeField] private RectTransform canvasRect;
    [SerializeField] private Button_Base sellButton;
    [SerializeField] private GameObject cardSlotPrefab;
    [SerializeField] private GridLayoutGroup cardSlotLayout;
    private TowerDataHolder selectedTower;
    public event Action<int> OnSellTower;
    private List<CardSlotUI> activeCards = new();
    public event Action<SkillData> OnSellCard;
    private void Start()
    {
        TowerHoverable.OnHoverTowerStatic += OnHoverTower;
        TowerHoverable.OnLeaveTowerHoverStatic += OnLeaveTowerHover;
        TowerHoverable.OnClickTowerStatic += OnClickTower;
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            selectedTower = null;
            UI.SetActive(false);
        };
        sellButton.OnClick.AddListener(() =>
        {
            OnSellTower?.Invoke(selectedTower.GoldValue);
            selectedTower.GetComponent<ISellable>().Sell();
            selectedTower = null;
            UI.SetActive(false);
        });
        UI.SetActive(false);
    }

    private void OnClickTower(TowerDataHolder towerData)
    {
        //if clicked tower is the same as the previous clicked tower
        if (selectedTower == towerData)
        {
            UI.SetActive(false);
            selectedTower = null;
            return;
        }

        selectedTower = towerData;
        ShowUI(towerData);
        selectedTower.OnUpdateStats += () =>
        {
            ShowUI(selectedTower);
        };
    }


    private void OnHoverTower(TowerDataHolder towerData)
    {
        if (selectedTower != null) return;
        ShowUI(towerData);
    }
    
    private void ShowUI(TowerDataHolder towerData)
    {
        UI.SetActive(true);
        foreach (var card in activeCards)
        {
            if(card != null)
                Destroy(card.gameObject);
        }
            
        activeCards.Clear();
        
        PositionUI(towerData.transform.position);
        nameText.text = towerData.Data.name;
        levelText.text = towerData.Level.ToString();
        damageText.text = "Base Damage : " + (towerData.Data.damage + towerData.ProjectileData.damage);
        fireRateText.text = "Fire Rate : " + (1f / towerData.RuntimeData.timeBetweenShots).ToString("F2") + "/sec";
        DPSText.text = "Real DPS : " + (towerData.RealDPS).ToString("F2");
        enemiesKilledText.text = "Enemies Killed : " + towerData.EnemiesKilled;
        totalDamageText.text = "Total Damage : " + towerData.TotalDamage;
        sellPriceText.text = "$" + towerData.GoldValue;

        foreach (var skill in towerData.SkillInstanceList)
        {
            var card = Instantiate(cardSlotPrefab, cardSlotLayout.transform).GetComponent<CardSlotUI>();
            card.Init(skill);
            activeCards.Add(card);
            card.OnSellCard += () =>
            {
                OnSellCard?.Invoke(card.SkillData);
                activeCards.Remove(card);
            };
        }
    }
    private void PositionUI(Vector3 worldPosition)
    {
        RectTransform uiRect = UI.GetComponent<RectTransform>();
        //Canvas canvas = canvasRect.GetComponent<Canvas>();

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        Vector2 offset = new Vector2(350, 0);
        Vector2 desiredScreenPos = screenPos + offset;

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
           // canvasRect, desiredScreenPos, null, out Vector2 anchoredPos);

        Vector2 halfSize = uiRect.rect.size / 2f;
        //Vector2 canvasSize = canvasRect.rect.size;

        //float minX = -canvasSize.x / 2f + halfSize.x;
        //float maxX = canvasSize.x / 2f - halfSize.x;
        //float minY = -canvasSize.y / 2f + halfSize.y;
        //float maxY = canvasSize.y / 2f - halfSize.y;

        //anchoredPos.x = Mathf.Clamp(anchoredPos.x, minX, maxX);
        //anchoredPos.y = Mathf.Clamp(anchoredPos.y, minY, maxY);

        //uiRect.anchoredPosition = anchoredPos;
    }
    private void OnLeaveTowerHover()
    {
        if (selectedTower != null) return;
        UI.SetActive(false);
    }

    private void OnDestroy()
    {
        TowerHoverable.OnHoverTowerStatic -= OnHoverTower;
        TowerHoverable.OnLeaveTowerHoverStatic -= OnLeaveTowerHover;
        TowerHoverable.OnClickTowerStatic -= OnClickTower;
    }
    */
}