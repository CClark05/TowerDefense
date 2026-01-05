using System;
using System.Linq;
using UnityEngine;

public class TowerCards : MonoBehaviour, IUsesCards
{ 
    public event Action<SkillInstance> OnAddedCard;
    public event Action<SkillInstance> OnRemovedCard;
    [SerializeField] private SkillRegistry skillRegistry;
    private TowerDataHolder towerDataHolder;
    private void Start()
    {
        towerDataHolder = GetComponent<TowerDataHolder>();
    }

    public void AddCard(SkillInstance skillInstance)
    {
        OnAddedCard?.Invoke(skillInstance);
        skillRegistry.AddNewSkill(skillInstance.Data);
    }


    public void RemoveCard(SkillInstance skillData)
    {
        OnRemovedCard?.Invoke(skillData);
        
    }

    public bool CanAddCard(SkillData skillData)
    {
        return !(towerDataHolder.SkillInstanceList.Any(s => s.Data == skillData) || towerDataHolder.SkillInstanceList.Count >= towerDataHolder.RuntimeData.CardSlots);
    }
}