using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Perfect Form Data", menuName = "SkillData/Crits/PerfectForm")]
public class PerfectFormSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Your first hit each round will always deal Critical damage for +{CritStats.baseCritMult * 100}% damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new PerfectFormSkillInstance(this);
    }
}

public class PerfectFormSkillInstance : SkillInstance<PerfectFormSkillData>, IHitModifier
{
    public PerfectFormSkillInstance(PerfectFormSkillData data) : base(data)
    {
    }

    public void Modify(HitData hitData, IDamageable target)
    {
        if (hitData.tower.ShotsThisRound == 1)
        {
            CritStats critStats = new CritStats();
            foreach (var mod in skillContext.GetSkillInstancesWith<ICritModifier>())
            {
                for (int i = 0; i < mod.instance.PlayCount; i++)
                    critStats.MultIncrease += mod.modifier.CritStats.MultIncrease;
            }
            critStats.DealCrit(hitData);
            PlayCard();
        }
    }
}
