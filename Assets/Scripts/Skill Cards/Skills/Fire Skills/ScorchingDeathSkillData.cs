using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Scorching Death Data", menuName = "SkillData/Fire/Scorching Death")]
public class ScorchingDeathSkillData : SkillData
{
    public int fireOnKill = 5;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Applies {fireOnKill} <color=#{hex}>Fire</color> on kill to all enemies.";
    }

    public override SkillInstance CreateInstance()
    {
        return new ScorchingDeathSkillInstance(this);
    }
}

public class ScorchingDeathSkillInstance : SkillInstance<ScorchingDeathSkillData>, IOnKill
{
    public ScorchingDeathSkillInstance(ScorchingDeathSkillData data) : base(data)
    {
    }

    public void OnKill(HitData hitData)
    {
        var enemies = EnemyManager.Instance.CurrentEnemies.Select(e => e.GetComponent<IUsesStatusEffects>()).ToList();
        foreach (var enemy in enemies)
        {
            //var onEffects = skillContext.GetSkillInstancesWith<IOnEffectApplied>().Where(e => e.modifier.Effect == Data.statusEffects[0].data);
            var effectData = new ModifyEffectData();
            CallModifier.Call<IOnEffectApplied>(skillContext, (mod, instance) =>
            {
                if(mod.Effect != Data.statusEffects[0].data) return;
                mod.Modify(effectData, hitData);
            });
            /**
            foreach (var mod in onEffects)
            {
                for(int i = 0; i < mod.instance.PlayCount; i++)
                    mod.modifier.Modify(effectData, hitData);
            }
            */
            enemy.AddPersistentEffect(Data.statusEffects[0].data as PersistentStatusEffect, hitData, Data.fireOnKill, effectData, hitData.ghost);
            PlayCard();
        }
    }
}