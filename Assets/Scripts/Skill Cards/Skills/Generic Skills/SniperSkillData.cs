using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "SkillData/Generic/Sniper")]
public class SniperSkillData : SkillData
{
    public int damagePerSecond = 10;

    private void OnValidate()
    {
        description = $"Projectiles gain +{damagePerSecond} damage per second in the air.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SniperSkillInstance(this);
    }
}

public class SniperSkillInstance : SkillInstance<SniperSkillData>, IProjectileModifier, IOnHit
{
    private ProjectileShotData projectileData;
    public SniperSkillInstance(SniperSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        PlayCard();
        projectileData = shotData;
        yield return null;
    }
    public void OnHit(HitData hitData)
    {
        PlayCard();
        hitData.finalDamage += Mathf.FloorToInt(projectileData.AirTime * Data.damagePerSecond);
    }
    public bool DelayShot { get; }
    
}
