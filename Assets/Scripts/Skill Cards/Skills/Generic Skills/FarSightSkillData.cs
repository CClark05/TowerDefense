using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Far Sight Data", menuName = "SkillData/Generic/FarSight")]
public class FarSightSkillData : SkillData
{
    public int increasedRange;

    private void OnValidate()
    {
        description = $"Gives +{increasedRange} range.";
    }

    public override SkillInstance CreateInstance()
    {
        return new FarSightSkillInstance(this);
    }
}

public class FarSightSkillInstance : SkillInstance<FarSightSkillData>, ITowerCardReceivedModifier
{
    public FarSightSkillInstance(FarSightSkillData data) : base(data)
    {
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedRange += Data.increasedRange;
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedRange -= Data.increasedRange;
    }
    
}