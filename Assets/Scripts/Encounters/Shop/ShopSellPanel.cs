using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopSellPanel : Singleton<ShopSellPanel>
{
    [SerializeField] private Button_Base returnButton;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GridLayoutGroup cardLayoutGroup;
    private ObservableHashSet<(SkillInstance instance, IUsesCards owner)> sellableCards = new();
    [SerializeField] private GameObject cardPrefab;
    public event Action<SkillInstance> OnSellCard;

    private new void Awake()
    {
        base.Awake();
        returnButton.OnClick.AddListener(() => { gameObject.SetActive(false); });
        sellableCards.OnItemAdded += item => AddCard(item.instance, item.owner);
    }

    private void OnEnable()
    {
        ShowCards();
    }

    private void ShowCards()
    {
        sellableCards.UnionWith(InventoryUI.Instance.SkillCards.Select(s => (s.SkillInstance, InventoryUI.Instance as IUsesCards)));
        TowerDataHolder.ActiveTowerList.ForEach(t => 
        {
            Debug.Log("test");
            sellableCards.UnionWith(t.SkillInstanceList.Select(instance => (instance, t.GetComponent<IUsesCards>())));
        });
    }

    private void AddCard(SkillInstance instance, IUsesCards owner)
    {
        var newCard = Instantiate(cardPrefab, cardLayoutGroup.transform);
        newCard.GetComponent<SetCardData>().SetData(instance.Data);
        newCard.GetComponent<SetCardData>().SetInstance(instance);
        newCard.GetComponent<SellCardUI>().OnSellCard += () =>
        {
            OnSellCard?.Invoke(instance);
            owner.RemoveCard(instance);
        };
        
        
    }

}
