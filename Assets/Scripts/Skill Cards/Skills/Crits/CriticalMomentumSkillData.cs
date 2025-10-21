using UnityEngine;

[CreateAssetMenu(fileName = "Critical Momentum Data", menuName = "SkillData/Crits/CriticalMomentum")]
public class CriticalMomentumSkillData : SkillData
{
    public int plusBaseDamage = 1;

    private void OnValidate()
    {
        description = $"Gains +{plusBaseDamage} permanent base damage on Critical kill. Resets if card is removed.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CriticalMomentumSkillInstance(this);
    }
}

public class CriticalMomentumSkillInstance : SkillInstance<CriticalMomentumSkillData>, IOnKill, IOnHit
{
    private int accumulatedBonus;
    public CriticalMomentumSkillInstance(CriticalMomentumSkillData data) : base(data)
    {
    }
    public void OnHit(HitData hitData)
    {
        PlayCard();
        hitData.finalDamage += accumulatedBonus;
    }
    public void OnKill(HitData hitData)
    {
        if (hitData.didCrit)
        {
            PlayCard();
            accumulatedBonus += Data.plusBaseDamage;
            //Modify(skillContext.Tower.RuntimeData);
        }
    }
}