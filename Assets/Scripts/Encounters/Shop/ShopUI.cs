using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private HorizontalLayoutGroup cardLayout;
    [SerializeField] private SkillRegistry skillRegistry;
    [SerializeField] private CardRaritySettings raritySettings;
    [SerializeField] private SOEvent onShowShopUI, onShowShopVisual;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button_Scale leaveButton, sellButton;
    [SerializeField] private Transform chestLocation;
    [SerializeField] private ShopSellPanel sellPanel;
    [SerializeField] private SkillData wildCardData, overflowData;
    public Transform ChestLocation => chestLocation;
    private int cardCount = 3;
    private CardCooldowns cardCooldowns = new(6);
    private List<SkillData> currentCards = new();
    private void Start()
    {
        leaveButton.OnClick.AddListener(() =>
        {
            canvas.gameObject.SetActive(false);
        });
        onShowShopUI.OnRaised += (object sender) =>
        {
            canvas.gameObject.SetActive(true);
        };
        onShowShopVisual.OnRaised += (object sender) =>
        {
            foreach (Transform child in cardLayout.transform)
                Destroy(child.gameObject);
            
            currentCards.Clear();
            currentCards = GenerateCards();
            foreach (SkillData data in currentCards)
            {
                GameObject newCard = Instantiate(cardPrefab, cardLayout.transform);
                newCard.GetComponent<SetCardData>().SetData(data);
            }
            Instantiate(cardPrefab, cardLayout.transform).GetComponent<SetCardData>().SetData(wildCardData);
            Instantiate(cardPrefab, cardLayout.transform).GetComponent<SetCardData>().SetData(overflowData);
            sellPanel.ToggleCardButtons(true);
        };
        sellButton.OnClick.AddListener(() =>
        {
            sellPanel.gameObject.SetActive(true);
        });
    }

    private List<SkillData> GenerateCards()
    {
        cardCooldowns.DecreaseCooldowns();
        float ShopWeightMod(CardRarity r) => r switch
        {
            //48.8% / 40.7% / 10.5%
            CardRarity.Common     => 0.4f, 
            CardRarity.Rare       => 1.6f, 
            CardRarity.Legendary  => 1.9f, 
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null)
        };
        var availableSkills = skillRegistry.Skills.Where(skill =>
            skill.prerequisiteSkills.Length == 0 || skill.prerequisiteSkills.Any(pr => skillRegistry.CurrentSkills.Contains(pr))).ToHashSet();
        HashSet<SkillData> filteredSkills = availableSkills.Where(skill => !cardCooldowns.ContainsKey(skill)).ToHashSet();
        HashSet<SkillData> pool = filteredSkills.Count >= cardCount ? filteredSkills : skillRegistry.Skills.ToHashSet();
        List<SkillData> cards = CardRarityPicker.PickCards(pool.ToList(), raritySettings, cardCount, ShopWeightMod);
        currentCards.AddRange(cards);
        cardCooldowns.SetCooldowns(cards);
        return cards;
    }
    
    
}
