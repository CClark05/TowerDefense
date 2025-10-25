using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Catalyst Data", menuName = "SkillData/Utility/Catalyst")]
public class CatalystSkillData : SkillData
{
    private void OnValidate()
    {
        description = "The next card added to this tower will always play an additional time. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CatalystSkillInstance(this);
    }
}
public class CatalystSkillInstance : SkillInstance<CatalystSkillData>, IOnNewCardAdded, ISelfDestructs
{
    private int accumulatedBonus;
    public CatalystSkillInstance(CatalystSkillData data) : base(data)
    {
    }

    public void Modify(SkillInstance cardInstance, TowerWaveData towerWaveData)
    {
        PlayCard();
        cardInstance.PlayCount++;
        towerWaveData.removedCards.Add(Data);
        OnSelfDestruct?.Invoke();
    }

    public Action OnSelfDestruct { get; set; }
}