using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Killing Spree Data", menuName = "SkillData/Crits/KillingSpree")]
public class KillingSpreeSkillData : SkillData
{
    public float CritDamageGainOnKill = 0.1f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Gain +{CritDamageGainOnKill * 100}% <color=#{hex}>Critical</color> damage on kill. Resets after wave.";
    }

    public override SkillInstance CreateInstance()
    {
        return new KillingSpreeSkillInstance(this);
    }
}

public class KillingSpreeSkillInstance : SkillInstance<KillingSpreeSkillData>, IOnKill, ICritModifier
{
    public CritStats CritStats { get; }
    public KillingSpreeSkillInstance(KillingSpreeSkillData data) : base(data)
    {
        CritStats = new CritStats();
        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
    }

    private void OnWaveComplete()
    {
        CritStats.MultIncrease = 0;
        RuntimeStat = Mathf.FloorToInt(CritStats.MultIncrease * 100);
    }

    public void OnKill(HitData hitData)
    {
        CritStats.MultIncrease += Data.CritDamageGainOnKill;
        RuntimeStat = Mathf.FloorToInt(CritStats.MultIncrease * 100);
        PlayCard();
    }
    public override void Dispose()
    {
        base.Dispose();
        EnemyManager.Instance.OnWaveComplete -= OnWaveComplete;
    }
    
}
