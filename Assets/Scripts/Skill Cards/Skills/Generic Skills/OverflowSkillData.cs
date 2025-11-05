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

public class OverflowSkillInstance : SkillInstance<OverflowSkillData>, ITowerCardReceivedModifier
{
    public OverflowSkillInstance(OverflowSkillData data) : base(data)
    {
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        PlayCard();
        towerWaveData.increasedSlots += Data.extraSlots;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSlots -= Data.extraSlots;
    }

    public bool alwaysPlayOnce { get; }
}