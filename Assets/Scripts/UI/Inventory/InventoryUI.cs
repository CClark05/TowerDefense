using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class InventoryUI : Singleton<InventoryUI>, IUsesCards
{
    [SerializeField] private Button_Hover chestButton;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private VerticalLayoutGroup cardLayoutGroup;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SkillRegistry skillRegistry;
    public List<SkillCardUI> SkillCards { get; private set; } = new();
    public event Action OnRemovedCard;
    
    public List<SkillData> testingData; //REMOVE THIS LATER
    [SerializeField] private InventorySettings settings;
    private void Start()
    {
        InventoryChestUI chestUI = chestButton.GetComponent<InventoryChestUI>();
        BuildingManager.Instance.OnEnterBuildMode += EnterBuildMode;
        BuildingManager.Instance.OnExitBuildMode += ExitBuildMode;
        void ExitBuildMode()
        {
            if (chestUI.RemainOpen) return;
            inventoryUI.SetActive(false);
            chestUI.SetState(InventoryChestUI.States.Closed);
        }

        void EnterBuildMode()
        {
            chestUI.SetState(InventoryChestUI.States.Open);
            inventoryUI.SetActive(true);
        }
        chestButton.OnHover += () =>
        {
            if (chestUI.RemainOpen || BuildingManager.Instance.IsInBuildMode) return;
            chestUI.SetState(InventoryChestUI.States.Peeking);
            inventoryUI.SetActive(true);
        };
        chestButton.OnLeaveHover += () =>
        {
            if (chestUI.RemainOpen || BuildingManager.Instance.IsInBuildMode) return;
            inventoryUI.SetActive(false);
            chestUI.SetState(InventoryChestUI.States.Closed);
        };
        chestButton.OnClick.AddListener(() =>
        {
            chestUI.ToggleRemainOpen(!chestUI.RemainOpen);
            chestUI.SetState(chestUI.RemainOpen ? InventoryChestUI.States.Open : InventoryChestUI.States.Closed);
            inventoryUI.SetActive(chestUI.RemainOpen);
        });

        EnemyManager.Instance.OnWaveStarted += () =>
        {
            inventoryUI.SetActive(false);
            chestUI.gameObject.SetActive(false);
        };
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            inventoryUI.SetActive(true);
            chestUI.gameObject.SetActive(true);
        };
    }
    //REMOVE THIS
    private int test = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAddCard(testingData[test]);
            test++;
        }
    }
    
    public bool TryAddCard(SkillData data)
    {
        int maxCards = settings.MaxCards;
        if (SkillCards.Count >= maxCards) return false;
        SkillCardUI skillCard = Instantiate(cardPrefab, cardLayoutGroup.transform).GetComponent<SkillCardUI>();
        skillCard.transform.SetAsLastSibling();
        var canvas = skillCard.GetComponent<Canvas>();
        int maxOrder = 0;
        foreach (Transform t in cardLayoutGroup.transform)
        {
            var c = t.GetComponent<Canvas>();
            if (c && c.overrideSorting) maxOrder = Mathf.Max(maxOrder, c.sortingOrder);
        }
        canvas.sortingOrder = maxOrder + 1; 
        skillCard.GetComponent<SetCardData>().SetData(data);
        SkillCards.Add(skillCard);
        skillRegistry.AddNewSkill(data);
        skillCard.OnRemoveCard += cardUI =>
        {
            SkillCards.Remove(cardUI);
            Destroy(skillCard.gameObject);
            OnRemovedCard?.Invoke();
        };
        return true;
    }

    public void RemoveCard(SkillData skillData)
    {
        var card = SkillCards.FirstOrDefault(c => c.SkillData == skillData);
        if (card != null)
        {
            SkillCards.Remove(card);
            Destroy(card.gameObject);
            OnRemovedCard?.Invoke();
        }
    }

    public bool CanAddCard(SkillData skillData)
    {
        return SkillCards.Count < settings.MaxCards;
    }
}
