using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Overclock Data", menuName = "SkillData/Generic/Overclock")]
public class OverclockSkillData : SkillData
{
    public int extraSlots;
    public float destroyChance;
    private void OnValidate()
    {
        description = $"Gives +{extraSlots} card slots on receiving this card. Has a {destroyChance * 100}% chance to self destruct at the end of the wave.";
    }
    public override SkillInstance CreateInstance()
    {
        return new OverclockSkillInstance(this);
    }
}
public class OverclockSkillInstance : SkillInstance<OverclockSkillData>, ITowerCardReceivedModifier, ITowerWaveEndModifier
{
    public OverclockSkillInstance(OverclockSkillData data) : base(data)
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

    public void Modify(TowerWaveData towerWaveData)
    {
        if(UnityEngine.Random.value < Data.destroyChance)
        {
            Debug.Log("Overclock self destruct");
            towerWaveData.removedCards.Add(Data);
        }
    }
}