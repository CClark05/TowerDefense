using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : Singleton<InventoryUI>, IUsesCards
{
    [SerializeField] private Button_Hover chestButton;
    [SerializeField] private GameObject inventoryUI;
    //[SerializeField] private VerticalLayoutGroup cardLayoutGroup;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SkillRegistry skillRegistry;
    [SerializeField] private InventoryCardSlot[] cardSlots;
    [SerializeField] private Transform handTransform;
    public List<SkillCardUI> SkillCards { get; private set; } = new();
    public event Action OnRemovedCard;
    
    public List<SkillData> testingData; //REMOVE THIS LATER
    [SerializeField] private InventorySettings settings;
    private bool handShown = true;
    private void Start()
    {
        Image handOverlay = chestButton.transform.GetChild(0).GetComponent<Image>();
        chestButton.OnHover += () =>
        {
            handOverlay.DOFade(80f / 255, 0.2f);
        };
        chestButton.OnLeaveHover += () =>
        {
            handOverlay.DOFade(0, 0.2f);
        };
        chestButton.OnClick.AddListener(ToggleHandVisibility);

        EnemyManager.Instance.OnWaveStarted += () =>
        {
            if(handShown) ToggleHandVisibility();
        };
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            if(!handShown)
                ToggleHandVisibility();
        };
    }

    private void ToggleHandVisibility()
    {
        float stagger = 0.05f;
        handShown = !handShown;
        if (!handShown)
        {
            for (int i = 0; i < SkillCards.Count; i++)
            {
                SkillCards[i]
                    .GetComponent<InventoryCardAnimation>()
                    .AnimateIntoHand(handTransform, i * stagger);
            }
            return;
        }
        for (int i = 0; i < SkillCards.Count; i++)
        {
            float reversedDelay = (SkillCards.Count - 1 - i) * stagger;

            SkillCards[i]
                .GetComponent<InventoryCardAnimation>()
                .AnimateBack(reversedDelay);
        }
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
