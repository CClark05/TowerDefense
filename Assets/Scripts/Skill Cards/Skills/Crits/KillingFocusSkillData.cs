using UnityEngine;

[CreateAssetMenu(fileName = "Killing Focus Data", menuName = "SkillData/Crits/KillingFocus")]
public class KillingFocusSkillData : SkillData
{
    public float CritGainOnKill = 0.05f;
    private void OnValidate()
    {
        description = $"Gain +{CritGainOnKill * 100}% Critical chance on kill. Resets after wave.";
    }

    public override SkillInstance CreateInstance()
    {
        return new KillingFocusSkillInstance(this);
    }
}

public class KillingFocusSkillInstance : SkillInstance<KillingFocusSkillData>, IOnKill, ICritModifier
{
    public CritStats CritStats { get; }
    public KillingFocusSkillInstance(KillingFocusSkillData data) : base(data)
    {
        CritStats = new CritStats();
        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
    }

    private void OnWaveComplete() => CritStats.ChanceIncrease = 0;

    public void OnKill(HitData hitData)
    {
        CritStats.ChanceIncrease += Data.CritGainOnKill;
        PlayCard();
    }

    public override void Dispose()
    {
        base.Dispose();
        EnemyManager.Instance.OnWaveComplete -= OnWaveComplete;
    }
}