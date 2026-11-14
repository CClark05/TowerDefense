using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tent Data", menuName = "SkillData/Tower Modifiers/Tent")]
public class TentSkillData : SkillData
{
    public float fireRateIncrease = 0.25f;
    private void OnValidate()
    {
        description = $"Towers in range gain +{fireRateIncrease * 100f}% fire rate. This tower cannot shoot projectiles.";
    }

    public override SkillInstance CreateInstance()
    {
        return new TentSkillInstance(this);
    }
}

public class TentSkillInstance : SupportSkillInstance<TentSkillData>
{
    public TentSkillInstance(TentSkillData data) : base(data)
    {
        OnApply += tower =>
        {
            if (tower.RuntimeData.TimeBetweenShots == -1f) return;
            tower.RuntimeData.TimeBetweenShots /= 1f + Data.fireRateIncrease;
        };
        OnRemove += tower =>
        {
            if (tower.RuntimeData.TimeBetweenShots == -1f) return;
            tower.RuntimeData.TimeBetweenShots *= 1f + Data.fireRateIncrease;
        };
    }
}
