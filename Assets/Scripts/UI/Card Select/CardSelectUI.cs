using System;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private TextMeshProUGUI rerollCostText;
    [SerializeField] private GameObject background;
    private List<GameObject> currentCards = new();
    private int rerollAmount;
    private int rerollCost;
    public event Action<int> OnReroll;
    public event Action OnSelectedCard;
    private void Start()
    {
        rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
        string originalColor = "#" + ColorUtility.ToHtmlStringRGB(rerollCostText.color);
        string redColor = "#A53030";
        SetPriceColor();
        GenerateRandomCards();

        rerollButton.OnClick.AddListener(() =>
        {
            if (PlayerInventory.Instance.Coins < rerollCost) return;
            OnReroll?.Invoke(rerollCost);
            rerollAmount++;
            rerollCost = rerollSettings.BaseCost + rerollSettings.IncreasePerRoll * rerollAmount;
            rerollCostText.text = $"${rerollCost}";
            GenerateRandomCards();
            SetPriceColor();
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
        });
        PlayerInventory.Instance.OnCoinsUpdated += coins =>
        {
            SetPriceColor();
        };
        void SetPriceColor()
        {
            string priceColor = PlayerInventory.Instance.Coins >= rerollCost ? originalColor : redColor;
            rerollCostText.text = $"<color={priceColor}>${rerollCost}</color>";
        }

        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
        background.SetActive(false);
    }
    private void OnWaveComplete()
    {
        float delay = 1.5f;
        FunctionTimer.Create(() =>
        {
            background.SetActive(true);
            peekButton.gameObject.SetActive(true);
            GenerateRandomCards();
        }, delay);

    }

    private void GenerateRandomCards()
    {
        foreach (var card in currentCards)
        {
            Destroy(card);
        }

        currentCards.Clear();
        var availableSkills = skillRegistry.Skills.Where(skill =>
                skill.prerequisiteSkills.Length == 0 || skill.prerequisiteSkills.Any(pr => skillRegistry.CurrentSkills.Contains(pr))).ToHashSet();
        var filteredSkills = availableSkills.Where(skill => !skillRegistry.CurrentSkills.Contains(skill)).ToHashSet();
        var pool = filteredSkills.Count >= 3 ? filteredSkills : availableSkills;
        var cards = CardRarityPicker.PickCards(pool.ToList(), raritySettings, 3);
        foreach (var data in cards)
        {
            var newCard = Instantiate(cardPrefab, cardLayout.transform);
            newCard.GetComponent<SetCardData>().SetData(data);
            currentCards.Add(newCard);
            newCard.GetComponentInChildren<Button_Base>().OnClick.AddListener(() =>
            {
                if (InventoryUI.Instance.TryAddCard(data))
                {
                    background.SetActive(false);
                    peekButton.gameObject.SetActive(false);
                    OnSelectedCard?.Invoke();
                    return;
                }
                newCard.GetComponent<UIShake>().TriggerShake();
                InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Play();
                InventoryUI.Instance.OnRemovedCard += () =>
                {
                    InventoryChestUI.Instance.GetComponent<UIScaleLoop>().Stop();
                };
            });
        }
    }


}