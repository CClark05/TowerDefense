using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopSellPanel : Singleton<ShopSellPanel>
{
    internal class SellableCard : IEquatable<SellableCard>
    {
        public SkillInstance Instance { get; }
        public IUsesCards Owner { get; }
        public SellCardUI UI { get; set; }
        public SellableCard(SkillInstance instance, IUsesCards owner)
        {
            Instance = instance;
            Owner = owner;
        }
        public bool Equals(SellableCard other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ReferenceEquals(Instance, other.Instance);
        }
        public override int GetHashCode()
        {
            return Instance != null ? Instance.GetHashCode() : 0;
        }
    }
    
    [SerializeField] private Button_Base returnButton;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GridLayoutGroup cardLayoutGroup;
    private ObservableHashSet<SellableCard> sellableCards = new();
    [SerializeField] private GameObject cardPrefab;
    public event Action<SkillInstance> OnSellCard;
    private new void Awake()
    {
        base.Awake();
        returnButton.OnClick.AddListener(() => { gameObject.SetActive(false); });
        sellableCards.OnItemAdded += AddCard;
    }

    private void OnEnable()
    {
        ShowCards();
    }

    private void ShowCards()
    {
        List<Transform> children = cardLayoutGroup.transform.Cast<Transform>().ToList();
        foreach(var child in children)
        {
            if(child.GetComponent<ICardUI>() == null)
                Destroy(child.gameObject);
        }
        sellableCards.UnionWith(InventoryUI.Instance.SkillCards.Select(s => new SellableCard(s.SkillInstance, InventoryUI.Instance)));
        TowerDataHolder.ActiveTowerList.ForEach(t => 
        {
            sellableCards.UnionWith(t.SkillInstanceList.Select(instance => new SellableCard(instance, t.GetComponent<IUsesCards>())));
        });
    }

    private void AddCard(SellableCard card)
    {
        var newCard = Instantiate(cardPrefab, cardLayoutGroup.transform);
        card.UI = newCard.GetComponent<SellCardUI>();
        newCard.GetComponent<SetCardData>().SetData(card.Instance.Data);
        newCard.GetComponent<SetCardData>().SetInstance(card.Instance);
        newCard.GetComponent<SellCardUI>().OnSellCard += () =>
        {
            OnSellCard?.Invoke(card.Instance);
            card.Owner.RemoveCard(card.Instance);
            sellableCards.Remove(card);
            ToggleCardButtons(false);
        };
        card.Instance.OnDispose += () =>
        {
            sellableCards.Remove(card);
            Destroy(newCard);
        };
    }
    
    public void ToggleCardButtons(bool enable)
    {
        foreach(var card in sellableCards)
            card.UI.ToggleButton(enable);
    }
}
