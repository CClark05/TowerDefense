using System;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

[CreateAssetMenu(fileName = "Burning Haste", menuName = "SkillData/Fire/Burning Haste")]
public class BurningHasteSkillData : SkillData
{
    public float duration;
    public int hasteStacks = 1;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(buffs[0].data.color);
        description = $"Gain {hasteStacks} <color=#{hex}>{buffs[0].data.name}</color> for {duration} seconds on killing a burning enemy.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BurningHasteSkillInstance(this);
    }
}

public class BurningHasteSkillInstance : SkillInstance<BurningHasteSkillData>, IOnKill
{
    public BurningHasteSkillInstance(BurningHasteSkillData data) : base(data)
    {
    }

    public void OnKill(HitData hitData)
    {
        if(hitData.statusEffects.PersistentEffectsApplied.ContainsKey(Data.statusEffects[0].data as PersistentStatusEffect))
        {
            skillContext.AddBuff(Data.buffs[0].data as IBuff, Data.hasteStacks);
            PlayCard();
            FunctionTimer.Create(() =>
            {
                skillContext.TryRemoveBuff(Data.buffs[0].data as IBuff, Data.hasteStacks);
            }, Data.duration);
        }
    }
}