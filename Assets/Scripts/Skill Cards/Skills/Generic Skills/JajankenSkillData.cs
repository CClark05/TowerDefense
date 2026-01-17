using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Jajanken Data", menuName = "SkillData/Generic/Jajanken")]
public class JajankenSkillData : SkillData
{
    public float FireRateMult = 0.5f;
    [FormerlySerializedAs("Color")] public Color ProjectileColor;
    public int PlusDamage = 5;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Reduces fire rate by {FireRateMult * 100}% but gain +{PlusDamage} base damage and apply +1 <color=#{hex}>Stun</color> and on hit.";
    }

    public override SkillInstance CreateInstance()
    {
        return new JajankenSkillInstance(this);
    }
    
    
}

public class JajankenSkillInstance : SkillInstance<JajankenSkillData>, ITowerCardReceivedModifier, IProjectileModifier, IHitModifier, IPlayCountPolicy<IHitModifier>, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    public JajankenSkillInstance(JajankenSkillData data) : base(data)
    {
    }
    public IEnumerator Modify(ProjectileShotData shotData)
    {
        shotData.projectileColor = Data.ProjectileColor;
        yield return null;
    }
    public bool DelayShot { get; }
    
    public void Apply(TowerWaveData towerWaveData)
    {
        skillContext.Tower.RuntimeData.TimeBetweenShots /= 1 + Data.FireRateMult * PlayCount;
        towerWaveData.increasedBaseDamage += Data.PlusDamage * PlayCount;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        skillContext.Tower.RuntimeData.TimeBetweenShots *= 1 + Data.FireRateMult * PlayCount;
        towerWaveData.increasedBaseDamage -= Data.PlusDamage * PlayCount;
    }

    public void Modify(HitData hitData, IDamageable target)
    {
        PlayCard();
        (Data.statusEffects[0].data as StunStatusEffect).AddStacks(hitData, PlayCount);
        Damage += Data.PlusDamage * PlayCount;
    }

    public int SetPlayCount() => 1;
}
