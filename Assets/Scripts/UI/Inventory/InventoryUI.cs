using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        skillCard.OnRemoveCard += cardUI =>
        {
            SkillCards.Remove(cardUI);
            Destroy(skillCard.gameObject);
            OnRemovedCard?.Invoke();
            for(int i = 0; i<SkillCards.Count; i++)
            {
                var parent = cardSlots[i];
                if(SkillCards[i].transform.parent == parent.transform) continue;
                SkillCards[i].transform.SetParent(parent.transform);
                SkillCards[i].transform.localPosition = Vector3.zero;
            }
        };
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
        if (card != null)
        {
            SkillCards.Remove(card);
            Destroy(card.gameObject);
            OnRemovedCard?.Invoke();
            for(int i = 0; i<SkillCards.Count; i++)
            {
                var parent = cardSlots[i];
                if(SkillCards[i].transform.parent == parent.transform) continue;
                SkillCards[i].transform.SetParent(parent.transform);
                SkillCards[i].transform.localPosition = Vector3.zero;
            }
        }
        
    }

    public bool CanAddCard(SkillData skillData)
    {
        return SkillCards.Count < settings.MaxCards;
    }

    
}
