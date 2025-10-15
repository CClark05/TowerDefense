using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Execution Data", menuName = "SkillData/Crits/Execution")]
public class ExecutionSkillData : SkillData
{
    public float HealthThreshold = 0.10f;
    public float DamageMarkerSizeMult = 1.5f;

    private void OnValidate()
    {
        description = $"Critical hits instantly kill enemies below {HealthThreshold * 100}% health.";
    }

    public override SkillInstance CreateInstance()
    {
        return new ExecutionSkillInstance(this);
    }
}

public class ExecutionSkillInstance : SkillInstance<ExecutionSkillData>, IAfterHitModifier
{
    public ExecutionSkillInstance(ExecutionSkillData data) : base(data)
    {
    }

    public void OnHit(HitData hitData)
    {
       
    }

    public void Modify(HitData hitData)
    {
        if ((float)hitData.healthSystem.Health / hitData.healthSystem.MaxHealth <= Data.HealthThreshold && hitData.didCrit)
        {
            Debug.Log("execution");
            hitData.damageMarkerSizeMult *= Data.DamageMarkerSizeMult;
            hitData.finalDamage = 9999;
            PlayCard();
        }
    }
}
