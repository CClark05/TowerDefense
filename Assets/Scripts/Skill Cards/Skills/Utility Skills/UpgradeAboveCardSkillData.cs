using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Above Card Data", menuName = "SkillData/Utility/Upgrade Above Card")]
public class PromotionSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Upgrades the +1 card(s) above this one when added (if possible).";
    }

    public override SkillInstance CreateInstance()
    {
        return new UpgradeAboveCardSkillInstance(this);
    }
}
public class UpgradeAboveCardSkillInstance : SkillInstance<PromotionSkillData>, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    List<SkillInstance> upgradedCards = new List<SkillInstance>();
    public UpgradeAboveCardSkillInstance(PromotionSkillData data) : base(data)
    {
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        var cards = skillContext.Tower.SkillInstanceList.Where(c => c != this).ToList();
        if(cards.Count == 0) return;
        for (int i = cards.Count - 1; i >= 0 && upgradedCards.Count < PlayCount; i--)
        {
            var card = cards[i];
            if (card.Laminated) continue;
            card.PlayCount++;
            card.OnRemoveCard += OnCardRemoved;
            upgradedCards.Add(card);
        }
    }

    void OnCardRemoved(SkillInstance card)
    {
        card.PlayCount--;
        upgradedCards.Remove(card);
        card.OnRemoveCard -= OnCardRemoved;
        int index = skillContext.Tower.SkillInstanceList.IndexOf(this);
        var cards = skillContext.Tower.SkillInstanceList.Where(c => c != this && c != card).ToList();
        foreach (var c in cards)
        {
            Debug.Log(c.Data.name);
        }
        if(cards.Count == 0) return;
        for (int i = cards.Count - 1; i >= 0 && upgradedCards.Count < PlayCount && i < index - 1; i--)
        {
            var c = cards[i];
            if (c.Laminated || upgradedCards.Contains(c)) continue;
            c.PlayCount++;
            c.OnRemoveCard += OnCardRemoved;
            upgradedCards.Add(c);
        }
        
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        upgradedCards.ForEach(card =>
        {
            card.PlayCount--;
            card.OnRemoveCard -= OnCardRemoved;
        });
        upgradedCards.Clear();
    }
    
    public int SetPlayCount() => 1;
}