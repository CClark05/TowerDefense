using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Perfect Form Data", menuName = "SkillData/Crits/PerfectForm")]
public class PerfectFormSkillData : SkillData
{
    public int hits = 2;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Your first +{hits} hits each round will always deal <color=#{hex}>Critical</color> damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new PerfectFormSkillInstance(this);
    }
}

public class PerfectFormSkillInstance : SkillInstance<PerfectFormSkillData>, IHitModifier, ITowerCardReceivedModifier
{
    private int hits;
    public PerfectFormSkillInstance(PerfectFormSkillData data) : base(data)
    {
    }

    public void Modify(HitData hitData, IDamageable target)
    {
        if (hitData.tower.HitsThisRound < hits)
        {
            CritStats critStats = new CritStats();
            foreach (var mod in skillContext.GetSkillInstancesWith<ICritModifier>())
            {
                critStats.MultIncrease += mod.modifier.CritStats.MultIncrease;
            }
            critStats.DealCrit(hitData);
            PlayCard();
        }
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        hits += Data.hits;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        hits -= Data.hits;
    }

}
