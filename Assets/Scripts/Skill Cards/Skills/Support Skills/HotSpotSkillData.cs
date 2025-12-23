using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Hot Spot Data", menuName = "SkillData/Support/Hot Spot")]
public class HotSpotSkillData : SkillData
{
    public int AdditionalSlots = 1;
    private void OnValidate()
    {
        description = $"Towers in range gain +{AdditionalSlots} card slot(s).";
    }

    public override SkillInstance CreateInstance()
    {
        return new HotSpotSkillInstance(this);
    }
}

public class HotSpotSkillInstance : SupportSkillInstance<HotSpotSkillData>
{
    public HotSpotSkillInstance(HotSpotSkillData data) : base(data)
    {
        OnApply += tower =>
        {
            Debug.Log("test2");
            tower.RuntimeData.CardSlots += Data.AdditionalSlots * PlayCount;
        };
        OnRemove += tower =>
        {
            tower.RuntimeData.CardSlots -= Data.AdditionalSlots * PlayCount;
        };
    }
}