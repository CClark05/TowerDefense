using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public class InventoryUI : Singleton<InventoryUI>, IUsesCards
{
    [SerializeField] private Button_Hover chestButton;
    [SerializeField] private GameObject inventoryUI;
    //[SerializeField] private VerticalLayoutGroup cardLayoutGroup;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SkillRegistry skillRegistry;
    [SerializeField] private InventoryCardSlot[] cardSlots;
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
            AddCard(testingData[test].CreateInstance());
            test++;
        }
    }

    public Transform GetNextCardSlot() => cardSlots.FirstOrDefault(s => !s.IsOccupied)?.transform;

    public void AddCard(SkillInstance skillInstance)
    {
        Transform slotTransform = cardSlots.FirstOrDefault(s => !s.IsOccupied)?.transform;
        if (slotTransform == null)
        {
            Debug.LogError("No available card slots!");
            return;
        }
        SkillCardUI skillCard = Instantiate(cardPrefab, slotTransform).GetComponent<SkillCardUI>();
        skillCard.GetComponent<SetCardData>().SetData(skillInstance.Data);
        skillCard.GetComponent<SetCardData>().SetInstance(skillInstance);
        SkillCards.Add(skillCard);
        skillRegistry.AddNewSkill(skillInstance.Data);
        skillCard.OnRemoveCard += RemoveCard;
    }
    public void AddCards(List<SkillInstance> skillInstances)
    {
        StartCoroutine(routine());
        IEnumerator routine()
        {
            const float delay = 0.5f;
            
            foreach (var instance in skillInstances)
            {
                if (SkillCards.Count >= settings.MaxCards)
                    continue;
                yield return new WaitForSeconds(delay);
                AddCard(instance);
            }
        }
    }
    public void RemoveCard(SkillInstance skillData)
    {
        var card = SkillCards.FirstOrDefault(c => c.SkillInstance == skillData);
        RemoveCard(card);
    }
    private void RemoveCard(SkillCardUI card)
    {
        if (card != null)
        {
            SkillCards.Remove(card);
            Destroy(card.gameObject);
            OnRemovedCard?.Invoke();
            int counter = 0;
            for(int i = 0; i<SkillCards.Count; i++)
            {
                var parent = cardSlots[i];
                if(SkillCards[i].transform.parent == parent.transform) continue;
                counter++;
                var c = SkillCards[i];
                var originalPosition = c.transform.position;
                c.transform.parent = parent.transform;
                c.transform.position = originalPosition;
                c.GetComponent<InventoryCardAnimation>().SlideOverAnimation(parent.transform.position, (counter - 1) * 0.03f);
            }
        }
    }
    
    public bool CanAddCard(SkillInstance instance)
    {
        return SkillCards.Count < settings.MaxCards;
    }
    public bool CanAddCard()
    {
        return SkillCards.Count < settings.MaxCards;
    }
}
