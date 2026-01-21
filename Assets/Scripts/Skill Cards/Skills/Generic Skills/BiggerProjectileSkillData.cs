using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Bigger Projectile Data", menuName = "SkillData/Generic/Bigger Projectile")]
public class BiggerProjectileSkillData : SkillData
{
    public float PlusSize = 0.25f;
    private void OnValidate()
    {
        description = $"Your projectiles are +{PlusSize * 100}% bigger.";
    }
    public override SkillInstance CreateInstance()
    {
        return new BiggerProjectileSkillInstance(this);
    }
}
public class BiggerProjectileSkillInstance : SkillInstance<BiggerProjectileSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public BiggerProjectileSkillInstance(BiggerProjectileSkillData data) : base(data)
    {
    }
    public IEnumerator Modify(ProjectileShotData shotData)
    {
        shotData.plusSizePercent += Data.PlusSize * PlayCount;
        yield return null;
    }
    public int SetPlayCount() => 1;
    public bool DelayShot { get; }
}