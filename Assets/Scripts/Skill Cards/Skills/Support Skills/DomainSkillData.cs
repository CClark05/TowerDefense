
using System;
using System.Collections.Generic;
using System.Linq;
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
public class DomainSkillInstance : SkillInstance<DomainSkillData>, IOnEnemyEnteredRange, IPlayCountPolicy<IOnEnemyEnteredRange>
{
    public DomainSkillInstance(DomainSkillData data) : base(data)
    {
    }
    private Dictionary<IDamageable, Action<int>> trackedEnemies = new();
    public void OnEnemyEnteredRange(IDamageable enemy)
    {
        enemy.ApplyDamageBonus(Data.damageBonus * PlayCount);
        enemy.OnTakeDamage += OnTakeDamage;
        void OnTakeDamage(int damage) => Damage += (int)Math.Floor(damage * enemy.Transform.GetComponent<EnemyHealth>().GetDamageBonus());
        trackedEnemies.Add(enemy, OnTakeDamage);
    }

    public void OnEnemyLeftRange(IDamageable enemy)
    {
        enemy.ApplyDamageBonus(-Data.damageBonus * PlayCount);
        enemy.OnTakeDamage -= trackedEnemies[enemy];
        if (!trackedEnemies.Remove(enemy))
        {
            Debug.LogError("NOT GOOD!!");
        }
    }

    public int SetPlayCount() => 1;
}
