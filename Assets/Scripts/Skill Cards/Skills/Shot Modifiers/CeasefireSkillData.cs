using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Ceasefire Data", menuName = "SkillData/Shot Modifiers/Ceasefire")]
public class CeasefireSkillData : SkillData
{
    public int PlusSlots = 3;
    private void OnValidate()
    {
        description = $"Tower gains {PlusSlots} card slots but can no longer shoot projectiles.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CeasefireSkillInstance(this);
    }
}
public class CeasefireSkillInstance : SkillInstance<CeasefireSkillData>, ITowerCardReceivedModifier
{
    public CeasefireSkillInstance(CeasefireSkillData data) : base(data)
    {
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSlots += Data.PlusSlots;
        skillContext.Tower.GetComponent<TowerShooting>().ToggleShooting(true);
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSlots -= Data.PlusSlots;
        skillContext.Tower.GetComponent<TowerShooting>().ToggleShooting(false);
    }
}
