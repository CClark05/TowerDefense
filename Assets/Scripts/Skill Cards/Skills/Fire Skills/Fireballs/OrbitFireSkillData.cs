using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "Orbit Fire Data", menuName = "SkillData/Fire/Orbit Bombs")]
public class OrbitFireSkillData : SkillData
{
    public int Fireballs = 3;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"At the start of each wave create +{Fireballs} <color=#{hex}>Fireballs</color> that orbit your tower.";
    }

    public override SkillInstance CreateInstance()
    {
        return new OrbitFireSkillInstance(this);
    }
}
public class OrbitFireSkillInstance : SkillInstance<OrbitFireSkillData>, ITowerWaveStartModifier, IPlayCountPolicy<ITowerWaveStartModifier>, ITowerWaveEndModifier, IPlayCountPolicy<ITowerWaveEndModifier>
{
    private List<Fireball> fireballs = new();
    public OrbitFireSkillInstance(OrbitFireSkillData data) : base(data)
    {
    }
    void ITowerWaveStartModifier.Modify(TowerWaveData towerWaveData)
    {
        SpawnFireballs(Data.Fireballs * PlayCount);
        PlayCard();
    }
    void ITowerWaveEndModifier.Modify(TowerWaveData towerWaveData)
    {
        foreach (var f in fireballs)
            Object.Destroy(f);
        fireballs.Clear();
    }
    
    void SpawnFireballs(int count, float radius = 2f)
    {
        Vector3 center = skillContext.Tower.transform.position;
        if (count <= 0) return;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            var fireball = Object.Instantiate((Data.statusEffects[0].data as CardSpawnableEffectData).prefab, center + offset, Quaternion.identity).GetComponent<Fireball>();
            fireball.Init(skillContext.Tower, radius, i * angleStep);
            fireball.OnDealtDamage += damage =>
            {
                Damage += damage;
            };
            fireballs.Add(fireball);
        }
    }

    public int SetPlayCount() => 1;
}