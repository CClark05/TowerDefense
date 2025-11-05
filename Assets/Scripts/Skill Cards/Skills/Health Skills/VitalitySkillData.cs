using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Vitality Data", menuName = "SkillData/Health/Vitality")]
public class VitalitySkillData : SkillData
{
    public int livesGranted = 3;
    private void OnValidate()
    {
        description = $"Grants +{livesGranted} lives upon using this card. Self destructs after use.";
    }
    public override SkillInstance CreateInstance()
    {
        return new VitalitySkillInstance(this);
    }
}
public class VitalitySkillInstance : SkillInstance<VitalitySkillData>, ITowerCardReceivedModifier, ISelfDestructs
{
    public VitalitySkillInstance(VitalitySkillData data) : base(data)
    {
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        PlayCard();
        PlayerLife.Instance.AddLives(Data.livesGranted * PlayCount);
        towerWaveData.removedCards.Add(this);
        OnSelfDestruct?.Invoke();
    }
    public void Remove(TowerWaveData towerWaveData)
    {
    }
    public bool alwaysPlayOnce { get; } = true;

    public Action OnSelfDestruct { get; set; }
}