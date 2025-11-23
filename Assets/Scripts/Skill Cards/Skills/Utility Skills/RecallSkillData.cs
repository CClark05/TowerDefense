using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Recall Data", menuName = "SkillData/Utility/Recall")]
public class RecallSkillData : SkillData
{
    private void OnValidate()
    {
        description = "Creates a copy of the next card added to this tower and adds it to your hand. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new RecallSkillInstance(this);
    }
}
public class RecallSkillInstance : SkillInstance<RecallSkillData>, ISelfDestructs, IOnNewCardAdded
{
    public RecallSkillInstance(RecallSkillData data) : base(data)
    {
    }

    public Action OnSelfDestruct { get; set; }
    public void Modify(SkillInstance cardInstance, TowerWaveData towerWaveData)
    {
        PlayCard();
        var newInstance = cardInstance.Data.CreateInstance();
        InventoryUI.Instance.AddCard(newInstance);
        towerWaveData.removedCards.Add(this);
        OnSelfDestruct?.Invoke();
    }
}
