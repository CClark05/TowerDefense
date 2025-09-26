using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Kinetic Shot", menuName = "SkillData/Generic/Kinetic Shot")]
public class KineticShotSkillData : SkillData
{
    public int damagePerSecond = 10;

    private void OnValidate()
    {
        description = $"Projectiles gain +{damagePerSecond} base damage per second in the air.";
    }

    public override SkillInstance CreateInstance()
    {
        return new KineticShotSkillInstance(this);
    }
}

public class KineticShotSkillInstance : SkillInstance<KineticShotSkillData>, IProjectileModifier, IOnHit
{
    private ProjectileShotData projectileData;
    public KineticShotSkillInstance(KineticShotSkillData data) : base(data)
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
        hitData.finalDamage += Mathf.CeilToInt(projectileData.AirTime * Data.damagePerSecond);
    }
    public bool DelayShot { get; }
    
}
