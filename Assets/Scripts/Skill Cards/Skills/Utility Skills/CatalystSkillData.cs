using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Catalyst Data", menuName = "SkillData/Utility/Catalyst")]
public class CatalystSkillData : SkillData
{
    private void OnValidate()
    {
        description = "The next +1 unupgraded card(s) added to this tower are permanently upgraded to level 2. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CatalystSkillInstance(this);
    }
}
public class CatalystSkillInstance : SkillInstance<CatalystSkillData>, IOnNewCardAdded, ISelfDestructs, IPlayCountPolicy<IOnNewCardAdded>
{
    private int cardsUpgraded;
    
    public CatalystSkillInstance(CatalystSkillData data) : base(data)
    {
    }

    public void Modify(SkillInstance cardInstance, TowerWaveData towerWaveData)
    {
        if (cardInstance.PlayCount != 1) return;
        PlayCard();
        cardInstance.PlayCount = 2;
        cardsUpgraded++;
        if(cardsUpgraded >= PlayCount)
            OnSelfDestruct?.Invoke();
    }

    public Action OnSelfDestruct { get; set; }
    public int SetPlayCount() => 1;
}