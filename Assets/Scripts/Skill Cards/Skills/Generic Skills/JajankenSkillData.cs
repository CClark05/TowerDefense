
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Jajanken Data", menuName = "SkillData/Generic/Jajanken")]
public class JajankenSkillData : SkillData
{
    public int ChargeTime = 2;
    [FormerlySerializedAs("DamageIncrease")] public float DamageMult = 0.5f;
    public Color Color;
    private void OnValidate()
    {
        description = $"Shots charge up for {ChargeTime} seconds before releasing for +{DamageMult * 100}% damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new JajankenSkillInstance(this);
    }
    
    
}

public class JajankenSkillInstance : SkillInstance<JajankenSkillData>, IProjectileModifier, IHitModifier
{
    public bool DelayShot => true;
    public JajankenSkillInstance(JajankenSkillData data) : base(data)
    {
    }
    public IEnumerator Modify(ProjectileShotData shotData)
    {
        PlayCard();
        shotData.projectileColor = Data.Color;
        yield return new WaitForSeconds(Data.ChargeTime);
    }
    public void Modify(HitData hitData, IDamageable target)
    {
        PlayCard();
        hitData.finalDamage = CalculateDamage.MultIncrease(Data.DamageMult, hitData.finalDamage);
    }
}
