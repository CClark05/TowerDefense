using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Gilded Greed Data", menuName = "SkillData/Economy/GildedGreed")]
public class GildedGreedSkillData : SkillData
{
    public int PlusDamage = 5;
    public int GoldPerIncrement = 25;
    private void OnValidate()
    {
        description = $"Each {GoldPerIncrement} gold you have grants +{PlusDamage} base damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new GildedGreedSkillInstance(this);
    }
}

public class GildedGreedSkillInstance : SkillInstance<GildedGreedSkillData>, IOnCoinsUpdated, ITowerCardReceivedModifier, IPlayCountPolicy<IOnCoinsUpdated>, IPlayCountPolicy<ITowerCardReceivedModifier>, IOnHit, IPlayCountPolicy<IOnHit>
{
    private int currentBonus;
    public GildedGreedSkillInstance(GildedGreedSkillData data) : base(data)
    {
        OnPlayCountUpdatedHandle += _ =>
        {
            skillContext.Tower.RuntimeData.BaseDamage -= currentBonus;
            CalculateBonus();
            skillContext.Tower.RuntimeData.BaseDamage += currentBonus;
        };
    }
    private Action<int> OnPlayCountUpdatedHandle;
    public void Apply(TowerWaveData towerWaveData)
    {
        OnPlayCountUpdated += OnPlayCountUpdatedHandle;
        CalculateBonus();
        skillContext.Tower.RuntimeData.BaseDamage += currentBonus;
        PlayCard();
    }
    public void Remove(TowerWaveData towerWaveData)
    {
        OnPlayCountUpdated -= OnPlayCountUpdatedHandle;
        skillContext.Tower.RuntimeData.BaseDamage -= currentBonus;
    }

    private int CalculateBonus()
    {
        int newBonus = PlayerInventory.Instance.Coins / Data.GoldPerIncrement * Data.PlusDamage * PlayCount; 
        int bonusDelta = (newBonus - currentBonus); 
        currentBonus = newBonus;
        RuntimeStat = currentBonus;
        return bonusDelta;
    }
    public void OnCoinsUpdated() => skillContext.Tower.RuntimeData.BaseDamage += CalculateBonus();
    public int SetPlayCount() => 1;
    public void OnHit(HitData hitData)
    {
        Damage += currentBonus;
    }
}