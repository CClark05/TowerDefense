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
public class OverclockSkillInstance : SkillInstance<OverclockSkillData>, ITowerCardReceivedModifier, ITowerWaveEndModifier, ISelfDestructs
{
    public Action OnSelfDestruct { get; set; }
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
            towerWaveData.removedCards.Add(this);
            OnSelfDestruct?.Invoke();
        }
    }

    
}