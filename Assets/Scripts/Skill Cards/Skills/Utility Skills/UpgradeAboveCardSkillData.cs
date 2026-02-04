using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Above Card Data", menuName = "SkillData/Utility/Upgrade Above Card")]
public class PromotionSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Upgrades the +1 most recently added card(s) to the tower (if possible).";
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