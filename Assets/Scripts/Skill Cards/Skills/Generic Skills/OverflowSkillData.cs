using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Overflow Data", menuName = "SkillData/Generic/Overflow")]
public class OverflowSkillData : SkillData
{
    public int extraSlots;

    private void OnValidate()
    {
        description = $"Gives +{extraSlots} card slots on receiving this card.";
    }

    public override SkillInstance CreateInstance()
    {
        return new OverflowSkillInstance(this);
    }
}

public interface IAddsCardSlots
{
}
public class OverflowSkillInstance : SkillInstance<OverflowSkillData>, ITowerCardReceivedModifier, IAddsCardSlots
{
    private Action<int> OnPlayCountUpdatedHandle;
    private int addedSlots;
    public OverflowSkillInstance(OverflowSkillData data) : base(data)
    {
        OnPlayCountUpdatedHandle += _ =>
        {
            int difference = (Data.extraSlots * PlayCount) - addedSlots;
            skillContext.Tower.RuntimeData.CardSlots += difference;
            addedSlots += difference;
        };
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        OnPlayCountUpdated += OnPlayCountUpdatedHandle;
        PlayCard();
        towerWaveData.increasedSlots += Data.extraSlots;
        addedSlots += Data.extraSlots;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        OnPlayCountUpdated -= OnPlayCountUpdatedHandle;
        towerWaveData.increasedSlots -= Data.extraSlots;
        addedSlots -= Data.extraSlots;
    }
    
}