using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Random Card Data", menuName = "SkillData/Utility/Upgrade Random Card")]
public class UpgradeRandomCardSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Upgrades +1 random card(s) on any of your towers. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new UpgradeRandomCardSkillInstance(this);
    }
}
public class UpgradeRandomCardSkillInstance : SkillInstance<UpgradeRandomCardSkillData>, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>, ISelfDestructs
{
    public UpgradeRandomCardSkillInstance(UpgradeRandomCardSkillData data) : base(data)
    {
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        List<SkillInstance> upgradedCards = new();
        
        var all = new List<SkillInstance>();
        foreach (var t in TowerDataHolder.ActiveTowerList)
            all.AddRange(t.SkillInstanceList);
        
        for (int i = 0; i < PlayCount; i++)
        {
            var upgradableCards = all.Where(c => c != this && !c.Laminated && !upgradedCards.Contains(c)).ToList();
            if (upgradableCards.Count == 0)
            {
                OnSelfDestruct?.Invoke();
                return;
            }
            var c = upgradableCards[UnityEngine.Random.Range(0, upgradableCards.Count)];
            c.PlayCount++;
            Debug.Log("Upgraded " + c.Data.name);
            upgradedCards.Add(c);
        }
        OnSelfDestruct?.Invoke();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        
    }

    public int SetPlayCount() => 1;
    public Action OnSelfDestruct { get; set; }
}