using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Bomb Data", menuName = "SkillData/Bomb/Create Bomb")]
public class CreateBombSkillData : SkillData
{
    public int KillsNeeded = 3;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Every {KillsNeeded} kills, create +1 <color=#{hex}>Bomb(s)</color> near where the enemy died.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CreateBombSkillInstance(this);
    }
}
public class CreateBombSkillInstance : SkillInstance<CreateBombSkillData>, ITowerWaveStartModifier, IOnKill, IPlayCountPolicy<IOnKill>
{
    private int killCounter;
    public CreateBombSkillInstance(CreateBombSkillData data) : base(data)
    {
    }

    public void Modify(TowerWaveData towerWaveData) => killCounter = 0;

    public void OnKill(HitData hitData)
    {
        killCounter++;
        if (killCounter % Data.KillsNeeded != 0) return;
        for (int i = 0; i < PlayCount; i++)
        {
            var bombData = Data.statusEffects[0].data as BombEffectData;
            var bomb = UnityEngine.Object.Instantiate(bombData.bombPrefab, hitData.damageable.Transform.position, Quaternion.identity).GetComponent<Bomb>();
            bomb.Init(skillContext.Tower);
            bomb.OnDealtDamage += damage =>
            {
                Damage += damage;
            };
        }
        PlayCard();
    }

    public int SetPlayCount() => 1;
}