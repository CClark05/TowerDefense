using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Devour Data", menuName = "SkillData/Utility/Devour")]
public class DevourSkillData : SkillData
{
    public int plusDamage = 2;
    private void OnValidate()
    {
        description = $"Destroys all cards on this tower and gains +{plusDamage} base damage for each card destroyed.";
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
        PlayCard();
        var cards = skillContext.Tower.SkillInstanceList.Where(c => c != this).ToList();
        accumulatedBonus += cards.Count * Data.plusDamage * PlayCount;
        towerWaveData.increasedBaseDamage += accumulatedBonus;
        towerWaveData.removedCards.AddRange(cards);
        RuntimeStat = accumulatedBonus;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= accumulatedBonus;
    }

    public int SetPlayCount() => 1;
}