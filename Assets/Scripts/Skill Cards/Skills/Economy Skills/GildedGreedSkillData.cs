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

public class GildedGreedSkillInstance : SkillInstance<GildedGreedSkillData>, IOnCoinsUpdated, ITowerCardReceivedModifier, IPlayCountPolicy<IOnCoinsUpdated>, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    private int currentBonus;
    public GildedGreedSkillInstance(GildedGreedSkillData data) : base(data)
    {
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        CalculateBonus();
        skillContext.Tower.RuntimeData.BaseDamage += currentBonus;
        PlayCard();
    }
    public void Remove(TowerWaveData towerWaveData) => skillContext.Tower.RuntimeData.BaseDamage -= currentBonus;

    private int CalculateBonus()
    {
        int newBonus = PlayerInventory.Instance.Coins / Data.GoldPerIncrement * Data.PlusDamage * PlayCount; 
        int bonusDelta = (newBonus - currentBonus); 
        currentBonus = newBonus; 
        return bonusDelta;
    }
    public void OnCoinsUpdated() => skillContext.Tower.RuntimeData.BaseDamage += CalculateBonus();
    public int SetPlayCount() => 1;
}