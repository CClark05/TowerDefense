using UnityEngine;

[CreateAssetMenu(fileName = "Focused Impact Data", menuName = "SkillData/Generic/FocusedImpact")]
public class FocusedImpactSkillData : SkillData
{
    public int PlusDamage = 2;
    
    private void OnValidate()
    {
        description = $"Adds +{PlusDamage} base damage on hit.";
    }

    public override SkillInstance CreateInstance()
    {
        return new FocusedImpactSkillInstance(this);
    }
}

public class FocusedImpactSkillInstance : SkillInstance<FocusedImpactSkillData>, IOnHit
{
    public void OnHit(HitData hitData)
    {
        PlayCard();
        hitData.finalDamage += Data.PlusDamage;
    }

    public FocusedImpactSkillInstance(FocusedImpactSkillData data) : base(data)
    {
    }
}

