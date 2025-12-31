using UnityEngine;

[CreateAssetMenu(fileName = "Killing Focus Data", menuName = "SkillData/Crits/KillingFocus")]
public class KillingFocusSkillData : SkillData
{
    public float CritGainOnKill = 0.05f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Gain +{CritGainOnKill * 100}% crit chance on <color=#{hex}>Critical</color> kill. Resets after wave.";
    }

    public override SkillInstance CreateInstance()
    {
        return new KillingFocusSkillInstance(this);
    }
}

public class KillingFocusSkillInstance : SkillInstance<KillingFocusSkillData>, IOnKill, IHitModifier, IPlayCountPolicy<IHitModifier>
{
    public CritStats CritStats { get; }
    public KillingFocusSkillInstance(KillingFocusSkillData data) : base(data)
    {
        CritStats = new CritStats();
        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
    }

    private void OnWaveComplete()
    {
        CritStats.ChanceIncrease = 0;
        RuntimeStat = Mathf.FloorToInt(CritStats.ChanceIncrease * 100);
    }

    public void OnKill(HitData hitData)
    {
        CritStats.ChanceIncrease += Data.CritGainOnKill;
        RuntimeStat = Mathf.FloorToInt(CritStats.ChanceIncrease * 100);
        PlayCard();
    }
    public void Modify(HitData hitData, IDamageable target)
    {
        CritStats critStats = new CritStats();
        critStats.CalculateBonus(skillContext);
        Debug.Log("Current Crit Chance: " + CritStats.CritChance);
        if (UnityEngine.Random.value < CritStats.CritChance)
            critStats.DealCrit(hitData);
        
    }
    public override void Dispose()
    {
        base.Dispose();
        EnemyManager.Instance.OnWaveComplete -= OnWaveComplete;
    }
    int IPlayCountPolicy<IHitModifier>.SetPlayCount() => 1;
}