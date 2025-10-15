using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Hunter's Mark Data", menuName = "SkillData/Crits/HuntersMark")]
public class HuntersMarkSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Gain a {CritStats.baseCritChance * 100}% chance for hits to deal Critical damage for +{CritStats.baseCritMult * 100}% damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new HuntersMarkSkillInstance(this);
    }
}

public class HuntersMarkSkillInstance : SkillInstance<HuntersMarkSkillData>, IHitModifier
{
    public HuntersMarkSkillInstance(HuntersMarkSkillData data) : base(data)
    {
    }

    public void Modify(HitData hitData, IDamageable target)
    {
        CritStats critStats = new CritStats();
        critStats.CalculateBonus(skillContext);
        Debug.Log(critStats.CritChance);
        if (UnityEngine.Random.value < critStats.CritChance)
        {
            critStats.DealCrit(hitData);
            PlayCard();
        }
    }
}

public interface ICritModifier
{
    public CritStats CritStats { get; }
}

public class CritStats
{
    public static readonly float baseCritChance = 0.15f;
    public static readonly float baseCritMult = 1f;
    public static readonly Color color = new Color(70 / 255f, 130 / 255f, 50 / 255f);
    public static readonly float damageMarkerSizeMult = 1.25f;

    public float ChanceIncrease;
    public float MultIncrease;
    public float CritChance => Mathf.Clamp01(baseCritChance + ChanceIncrease);
    public float CritMult => baseCritMult + MultIncrease;

    public void DealCrit(HitData hitData)
    {
        if (hitData.didCrit) return;
        hitData.finalDamage = CalculateDamage.MultIncrease(CritMult, hitData.finalDamage);
        hitData.colors.Add(color);
        hitData.damageMarkerPunchEffect = true;
        hitData.damageMarkerSizeMult *= damageMarkerSizeMult;
        hitData.didCrit = true;
    }

    public void CalculateBonus(SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<ICritModifier>())
        {
            MultIncrease += mod.modifier.CritStats.MultIncrease;
            ChanceIncrease += mod.modifier.CritStats.ChanceIncrease;
        }
    }
}