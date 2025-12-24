using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Coin Shot Data", menuName = "SkillData/Generic/Coin Shot")]
public class CoinShotSkillData : SkillData
{
    public float damageMult = 2;
    public Sprite coinSprite;
    private void OnValidate()
    {
        description = $"If possible, use coins as projectiles for +{damageMult * 100}% damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CoinShotSkillInstance(this);
    }
}
public class CoinShotSkillInstance : SkillInstance<CoinShotSkillData>, IProjectileModifier, IHitModifier, IPlayCountPolicy<IProjectileModifier>, IPlayCountPolicy<IHitModifier>
{
    public CoinShotSkillInstance(CoinShotSkillData data) : base(data)
    {
    }

    private bool shotCoin;
    public IEnumerator Modify(ProjectileShotData shotData)
    {
        if (PlayerInventory.Instance.Coins > 0)
        {
            shotData.projectileSprite = Data.coinSprite;
            PlayerInventory.Instance.SubtractCoins(1);
            shotCoin = true;
        }
        else
            shotCoin = false;
        yield return null;
    }

    public bool DelayShot { get; }
    public void Modify(HitData hitData, IDamageable target)
    {
        if (shotCoin)
        {
            hitData.finalDamage = CalculateDamage.MultIncrease(Data.damageMult, hitData.finalDamage, PlayCount);
            PlayCard();
        }
    }

    public int SetPlayCount() => 1;
}
