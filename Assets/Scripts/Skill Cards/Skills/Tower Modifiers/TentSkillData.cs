using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tent Data", menuName = "SkillData/Tower Modifiers/Tent")]
public class TentSkillData : SkillData
{
    public float fireRateIncrease = 0.25f;
    private void OnValidate()
    {
        description = $"Towers in range gain +{fireRateIncrease * 100f}% fire rate. Cannot shoot projectiles.";
    }

    public override SkillInstance CreateInstance()
    {
        return new TentSkillInstance(this);
    }
}

public class TentSkillInstance : SkillInstance<TentSkillData>
{
    public TentSkillInstance(TentSkillData data) : base(data)
    {
    }
}
