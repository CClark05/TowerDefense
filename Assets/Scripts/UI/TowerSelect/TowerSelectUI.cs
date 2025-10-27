using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSelectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, damageText, fireRateText, RealDPSText,AverageDPS,Uptime,enemiesKilledText, totalDamageText, sellText;
    [SerializeField] private Button_Base sellButton;
    [SerializeField] private GameObject UI;
    [SerializeField] private TowerDataHolder towerData;
    [SerializeField] private TowerHoverable towerHoverable;
    [SerializeField] private GameObject cardIconPrefab;
    [SerializeField] private GridLayoutGroup cardLayoutGroup;
    [SerializeField] private GameObject fullCardPreview;
    [SerializeField] private VerticalLayoutGroup statusEffectGroup;
    [SerializeField] private GridLayoutGroup emptySlotsLayoutGroup;
    [SerializeField] private GameObject emptySlotPrefab;
    private List<GameObject> emptySlots = new();
    private bool selected;
    private List<(SkillInstance instance, CardIconUI iconUI)> activeCards = new();
    private CardIconUI selectedCard;
    public static event Action<SkillData> OnSellCardStatic;
    public event Action<SkillInstance> OnSellCard;
    private EnemyManager enemyManager;
    private CardSelectUI cardSelectUI;
    private void Start()
    {
        enemyManager = EnemyManager.Instance;
        cardSelectUI = CardSelectUI.Instance;
        towerHoverable.OnHoverTower += () => { UI.SetActive(true); };
        towerHoverable.OnLeaveHoverTower += () =>
        {
            if (selected) return;
            UI.SetActive(false);
        };
        towerHoverable.OnClickTower += () =>
        {
            selected = !selected;
            if (selected)
            {
                UI.SetActive(true);
                return;
            }

            UI.SetActive(!UI.activeSelf);
        };
        sellButton.OnClick.AddListener(() =>
        {
            if (selectedCard == null)
            {
                towerData.GetComponent<TowerSellable>().Sell();
                selected = false;
                UI.SetActive(false);
                return;
            }

            var card = activeCards.FirstOrDefault(c => c.iconUI == selectedCard);
            OnSellCardStatic?.Invoke(card.instance.Data);
            OnSellCard?.Invoke(card.instance);
            card.instance.Dispose();
            activeCards.Remove(card);
            Destroy(selectedCard.gameObject);
            selectedCard = null;
            sellText.text = $"SELL : <color=#DE9E41>${towerData.GoldValue}</color>";
            fullCardPreview.SetActive(false);
        });
        towerData.OnUpdateCards += UpdateCards;
        towerData.OnUpdateCards += UpdateUI;
        enemyManager.OnWaveStarted += OnWaveStarted;
        enemyManager.OnWaveComplete += OnWaveComplete;
        towerData.OnUpdateStats += UpdateUI;
        UpdateUI();
        UpdateCards();
        UpdateEmptySlots(towerData.Data.cardSlots);
        towerData.RuntimeData.OnCardSlotsUpdated += UpdateEmptySlots;
        cardSelectUI.OnShowCards += OnWaveComplete;
        UI.SetActive(false);
    }
    

    private void UpdateUI()
    {
        nameText.text = towerData.Data.objectName;
        damageText.text = "Base Damage : " + (towerData.RuntimeData.BaseDamage + towerData.ProjectileData.damage);
        fireRateText.text = "Fire Rate : " + (1f / towerData.RuntimeData.TimeBetweenShots).ToString("F2") + "/sec";
        RealDPSText.text = "Realtime DPS : " + Math.Round(towerData.RealWaveDPS, MidpointRounding.AwayFromZero).ToString();
        AverageDPS.text = "Average DPS : " + Math.Round(towerData.AverageOverallDps, MidpointRounding.AwayFromZero).ToString();
        Uptime.text = $"Uptime : {towerData.UptimePercentage:P1}";
        enemiesKilledText.text = "Enemies Killed : " + towerData.EnemiesKilled;
        totalDamageText.text = "Total Damage : " + towerData.TotalDamage;
        sellText.text = $"SELL : <color=#DE9E41>${towerData.GoldValue}</color>";
    }

    private void UpdateEmptySlots(int slots)
    {
        while (emptySlots.Count < slots)
        {
            var slot = Instantiate(emptySlotPrefab, emptySlotsLayoutGroup.transform);
            emptySlots.Add(slot);
        }
        while (emptySlots.Count > slots)
        {
            var slot = emptySlots[^1];
            emptySlots.RemoveAt(emptySlots.Count - 1);
            Destroy(slot);
        }
    }
    private void UpdateCards()
    {
        var toRemove = activeCards.Where(card => !towerData.SkillContext.ActiveSkills.Contains(card.instance)).ToList();
        foreach (var (instance, iconUI) in toRemove)
        {
            Destroy(iconUI.gameObject);
            activeCards.Remove((instance, iconUI));
        }

        foreach (var instance in towerData.SkillContext.ActiveSkills)
        {
            if (activeCards.Any(c => c.instance == instance)) continue;
            var newCard = Instantiate(cardIconPrefab, cardLayoutGroup.transform).GetComponent<CardIconUI>();
            //Debug.Log("init");
            newCard.Init(instance, towerData.GetComponent<IUsesCards>());
            var button = newCard.Button;
            activeCards.Add((instance, newCard));
            newCard.GetComponent<CardIconDragDrop>().OnRemoveCard += () =>
            {
                activeCards.Remove((instance, newCard));
                fullCardPreview.SetActive(false);
            };
            button.OnHover += () =>
            {
                if (selectedCard != null) return;
                fullCardPreview.GetComponent<CardPreviewUI>().SetSkill(instance.Data);
                fullCardPreview.SetActive(true);
            };
            button.OnLeaveHover += () =>
            {
                if (selectedCard != null) return;
                fullCardPreview.SetActive(false);
            };
            button.OnClick.AddListener(() =>
            {
                if (EnemyManager.Instance.WaveState is EnemyManager.WaveStates.Spawning || EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning) return;
                if (selectedCard == newCard)
                {
                    newCard.Selected = false;
                    selectedCard = null;
                    sellText.text = $"SELL : <color=#DE9E41>${towerData.GoldValue}</color>";
                    return;
                }

                if (selectedCard != null)
                    selectedCard.Selected = false;

                newCard.Selected = true;
                selectedCard = newCard;
                fullCardPreview.GetComponent<CardPreviewUI>().SetSkill(instance.Data);
                sellText.text = $"SELL : <color=#DE9E41>${Mathf.FloorToInt(instance.Data.price * 0.5f)}</color>";
            });
        }
    }

    private void OnWaveStarted()
    {
        selected = false;
        if (selectedCard != null)
        {
            selectedCard.Selected = false;
            selectedCard = null;
        }

        UI.SetActive(false);
    }
    private void OnWaveComplete()
    {
        selected = false;
        selectedCard = null;
        UI.SetActive(false);
    }
    private void OnDisable()
    {
        enemyManager.OnWaveStarted -= OnWaveStarted;
        enemyManager.OnWaveComplete -= OnWaveComplete;
        cardSelectUI.OnShowCards -= OnWaveComplete;
    }
}