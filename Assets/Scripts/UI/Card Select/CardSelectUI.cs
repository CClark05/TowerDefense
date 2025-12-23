using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;

public class CardSelectUI : Singleton<CardSelectUI>
{
    [SerializeField] private SkillRegistry skillRegistry;
    [SerializeField] private CardRaritySettings raritySettings;
    [SerializeField] private RerollSettings rerollSettings;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private HorizontalLayoutGroup cardLayout;
    [SerializeField] private Button_Base rerollButton;
    [SerializeField] private Button_Base skipButton;
    [SerializeField] private Button_Base peekButton;
    [SerializeField] private TextMeshProUGUI rerollCostText, rerollText;
    [SerializeField] private GameObject background;
    private List<GameObject> currentCards = new();
    private CardCooldowns cardCooldowns = new(3);
    private int rerollAmount;
    private int rerollCost;
    public event Action<int> OnReroll;
    public event Action OnSelectedCard;
    public bool IsActive => background.activeSelf;
    public event Action OnShowCards;

    private void Start()
    {
        rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
        SetPriceText();
        GenerateRandomCards();
        rerollButton.OnClick.AddListener(() =>
        {
            if (PlayerInventory.Instance.Coins < rerollCost || rerollAmount >= rerollSettings.MaxRerolls) return;
            OnReroll?.Invoke(rerollCost);
            rerollAmount++;
            rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
            GenerateRandomCards();
            SetPriceText();
        });
        skipButton.OnClick.AddListener(() =>
        {
            background.SetActive(false);
            peekButton.gameObject.SetActive(false);
            InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Stop();
            OnSelectedCard?.Invoke();
        });
        peekButton.OnClick.AddListener(() =>
        {
            background.SetActive(!background.activeSelf);
            if (IsActive) OnShowCards?.Invoke();
        });
        PlayerInventory.Instance.OnCoinsUpdated += coins => { SetPriceText(); };


        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
        background.SetActive(false);
    }

    private void SetPriceText()
    {
        rerollButton.gameObject.SetActive(true);
        Color color = PlayerInventory.Instance.Coins >= rerollCost ? Color.white : ColorPicker.red;
        rerollCostText.text = $"${rerollCost}";
        rerollCostText.color = color;
        if (rerollAmount < rerollSettings.MaxRerolls) return;
        rerollButton.gameObject.SetActive(false);
    }

    private int counter;

    private void OnWaveComplete()
    {
        if(PlayerLife.Instance.CurrentLives <= 0) return;
        var encounter = EncounterGenerator.Instance.GetEncounter(EnemyManager.Instance.CurrentWave - 1);
        if (encounter != null) return;
        rerollAmount = 0;
        rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
        SetPriceText();
        cardCooldowns.DecreaseCooldowns();
        float delay = 2f;
        FunctionTimer.Create(() =>
        {
            background.SetActive(true);
            peekButton.gameObject.SetActive(true);
            GenerateRandomCards();
            OnShowCards?.Invoke();
        }, delay);
    }


    private void GenerateRandomCards()
    {
        foreach (var card in currentCards)
            Destroy(card);

        currentCards.Clear();
        var availableSkills = Enumerable.ToHashSet(skillRegistry.Skills.Where(skill =>
            skill.prerequisiteSkills.Length == 0 || skill.prerequisiteSkills.Any(pr => skillRegistry.CurrentSkills.Contains(pr))));
        var filteredSkills = Enumerable.ToHashSet(availableSkills.Where(skill => !cardCooldowns.ContainsKey(skill)));
        var pool = filteredSkills.Count >= 3 ? filteredSkills : availableSkills;
        var cards = CardRarityPicker.PickCards(pool.ToList(), raritySettings, 3);
        cardCooldowns.SetCooldowns(cards);
        foreach (var data in cards)
        {
            var newCard = Instantiate(cardPrefab, cardLayout.transform);
            newCard.GetComponent<SetCardData>().SetData(data);
            currentCards.Add(newCard);
            newCard.GetComponentInChildren<Button_Base>().OnClick.AddListener(() =>
            {
                if (InventoryUI.Instance.CanAddCard(data))
                {
                    var skillInstance = data.CreateInstance();
                    InventoryUI.Instance.AddCard(skillInstance);
                    background.SetActive(false);
                    peekButton.gameObject.SetActive(false);
                    OnSelectedCard?.Invoke();
                    return;
                }

                newCard.GetComponent<UIShake>().TriggerShake();
                InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Play();
                InventoryUI.Instance.OnRemovedCard += () => { InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Stop(); };
            });
        }
    }
}