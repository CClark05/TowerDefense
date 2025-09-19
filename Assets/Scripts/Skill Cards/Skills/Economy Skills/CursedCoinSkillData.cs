using UnityEngine;

[CreateAssetMenu(fileName = "Cursed Coin Data", menuName = "SkillData/Economy/Cursed Coin")]
public class CursedCoinSkillData : SkillData
{
    public int effectRequirement = 2;
    public int plusGold = 5;

    private void OnValidate()
    {
        description = $"Gain +${plusGold} if the enemy dies while affected by at least {effectRequirement} status effects.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CursedCoinSkillInstance(this);
    }
}

public class CursedCoinSkillInstance : SkillInstance<CursedCoinSkillData>, IOnKill
{
    public CursedCoinSkillInstance(CursedCoinSkillData data) : base(data)
    {
    }

    public void OnKill(HitData hitData)
    {
        if (hitData.statusEffects.PersistentEffectsApplied.Count >= Data.effectRequirement)
        {
            PlayCard();
            PlayerInventory.Instance.AddCoins(Data.plusGold);
        }
    }
}