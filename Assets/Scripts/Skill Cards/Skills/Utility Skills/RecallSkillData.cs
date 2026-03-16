using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Recall Data", menuName = "SkillData/Utility/Recall")]
public class RecallSkillData : SkillData
{
    private void OnValidate()
    {
        description = "Creates +1 copy(s) of the next card added to this tower and adds it to your hand. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new RecallSkillInstance(this);
    }
}
public class RecallSkillInstance : SkillInstance<RecallSkillData>, ISelfDestructs, IOnNewCardAdded, IPlayCountPolicy<IOnNewCardAdded>
{
    public RecallSkillInstance(RecallSkillData data) : base(data)
    {
    }

    public Action OnSelfDestruct { get; set; }
    public void Modify(SkillInstance cardInstance, TowerWaveData towerWaveData)
    {
        for (int i = 0; i < PlayCount; i++)
        {
            PlayCard();
            var newInstance = cardInstance.Data.CreateInstance();
            newInstance.PlayCount = cardInstance.PlayCount;
            InventoryUI.Instance.AddCard(newInstance);
        }

        OnSelfDestruct?.Invoke();
    }

    public int SetPlayCount() => 1;
}
