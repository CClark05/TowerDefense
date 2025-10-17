using System;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TowerCards : MonoBehaviour, IUsesCards
{
    public event Action<SkillData> OnAddedCard;
    public event Action<SkillData> OnRemovedCard;
    [SerializeField] private SkillRegistry skillRegistry;
    private TowerDataHolder towerDataHolder;
    private void Start()
    {
        towerDataHolder = GetComponent<TowerDataHolder>();
    }

    public bool TryAddCard(SkillData skillData)
    {
        if (towerDataHolder.SkillInstanceList.Any(s => s.Data == skillData) || towerDataHolder.SkillInstanceList.Count >= towerDataHolder.RuntimeData.CardSlots) return false;
        OnAddedCard?.Invoke(skillData);
        skillRegistry.AddNewSkill(skillData);
        return true;
    }

    public void RemoveCard(SkillData skillData)
    {
        OnRemovedCard?.Invoke(skillData);
    }

    public bool CanAddCard(SkillData skillData)
    {
        return !(towerDataHolder.SkillInstanceList.Any(s => s.Data == skillData) || towerDataHolder.SkillInstanceList.Count >= towerDataHolder.RuntimeData.CardSlots);
    }
}