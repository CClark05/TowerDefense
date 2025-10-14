using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gilded Greed Data", menuName = "SkillData/Economy/GildedGreed")]
public class GildedGreedSkillData : SkillData
{
    public int PlusDamage = 5;
    public int GoldPerIncrement = 25;
    private void OnValidate()
    {
        description = $"Each {GoldPerIncrement} gold you have grants +{PlusDamage} base damage on hit.";
    }

    public override SkillInstance CreateInstance()
    {
        return new GildedGreedSkillInstance(this);
    }
}

public class GildedGreedSkillInstance : SkillInstance<GildedGreedSkillData>, IOnHit
{
    public GildedGreedSkillInstance(GildedGreedSkillData data) : base(data)
    {
    }

    public void OnHit(HitData hitData)
    {
        int increments = PlayerInventory.Instance.Coins / Data.GoldPerIncrement;
        if(increments <= 0) return;
        hitData.finalDamage += increments * Data.PlusDamage;
        PlayCard();
    }
}
