using UnityEngine;
[CreateAssetMenu(fileName = "Domain Data", menuName = "SkillData/Support/Domain")]
public class DomainSkillData : SkillData
{
    public float damageBonus = 0.1f;
    private void OnValidate()
    {
        description = $"Enemies inside your range take +{damageBonus * 100}% more damage.";
    }
    public override SkillInstance CreateInstance()
    {
        return new DomainSkillInstance(this);
    }
}
public class DomainSkillInstance : SkillInstance<DomainSkillData>, IOnEnemyEnteredRange
{
    public DomainSkillInstance(DomainSkillData data) : base(data)
    {
    }

    public void OnEnemyEnteredRange(IDamageable enemy)
    {
        enemy.ApplyDamageBonus(Data.damageBonus);
    }

    public void OnEnemyLeftRange(IDamageable enemy)
    {
        enemy.ApplyDamageBonus(-Data.damageBonus);
    }
}
