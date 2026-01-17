using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Hunter's Mark Data", menuName = "SkillData/Crits/HuntersMark")]
public class HuntersMarkSkillData : SkillData
{
    public float CritChance = 0.075f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Gain a +{CritChance * 100}% chance for hits to be <color=#{hex}>Critical</color>";
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
        Debug.Log("Current Crit Chance: " + (Data.CritChance));
        if (UnityEngine.Random.value < Data.CritChance)
        {
            var damage = critStats.DealCrit(hitData);
            if (damage == null) return;
            PlayCard();
            Damage += damage.Value;
        }
    }

    
}

public interface ICritModifier
{
    public CritStats CritStats { get; }
}

public class CritStats
{
    public static readonly float baseCritMult = 2f;
    public static readonly Color color = new Color(70 / 255f, 130 / 255f, 50 / 255f);
    public static readonly float damageMarkerSizeMult = 1.25f;

    public float ChanceIncrease;
    public float MultIncrease;
    public float CritChance => Mathf.Clamp01(ChanceIncrease);
    public float CritMult => baseCritMult + MultIncrease;
    //returns damage delta 
    public int? DealCrit(HitData hitData)
    {
        if (hitData.didCrit) return null;
        var damage = CalculateDamage.MultIncrease(CritMult, hitData.baseDamage, 1);
        hitData.finalDamage += damage;
        hitData.colors.Add(color);
        hitData.damageMarkerPunchEffect = true;
        hitData.damageMarkerSizeMult *= damageMarkerSizeMult;
        hitData.didCrit = true;
        return damage;
    }

    public void CalculateBonus(SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<ICritModifier>())
        {
            MultIncrease += mod.modifier.CritStats.MultIncrease;
            //ChanceIncrease += mod.modifier.CritStats.ChanceIncrease;
        }
    }
}