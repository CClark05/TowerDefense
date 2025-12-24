using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Devour Data", menuName = "SkillData/Utility/Devour")]
public class DevourSkillData : SkillData
{
    public int plusDamage = 2;
    private void OnValidate()
    {
        description = $"Tries to destroy a random card on tower when added and gains +{plusDamage} base damage if successful.";
    }

    public override SkillInstance CreateInstance()
    {
        return new DevourSkillInstance(this);
    }
}

public class DevourSkillInstance : SkillInstance<DevourSkillData>, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    private int accumulatedBonus;
    public DevourSkillInstance(DevourSkillData data) : base(data)
    {
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        var cards = skillContext.Tower.SkillInstanceList.Where(c => c != this).ToList();
        if(cards.Count == 0) return;
        accumulatedBonus += Data.plusDamage * PlayCount;
        towerWaveData.increasedBaseDamage += accumulatedBonus;
        towerWaveData.removedCards.Add(cards[Random.Range(0, cards.Count)]);
        RuntimeStat = accumulatedBonus;
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= accumulatedBonus;
    }

    public int SetPlayCount() => 1;
}