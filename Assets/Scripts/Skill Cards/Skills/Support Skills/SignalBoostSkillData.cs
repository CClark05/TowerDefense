using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Signal Boost Data", menuName = "SkillData/Support/Signal Boost")]
public class SignalBoostSkillData : SkillData
{
    public float IncreasedRange = 1.25f;
    private void OnValidate()
    {
        description = $"Towers in range gain +{(IncreasedRange - 1) * 100}% range.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SignalBoostSkillInstance(this);
    }
}
public class SignalBoostSkillInstance : SupportSkillInstance<SignalBoostSkillData>
{
    public SignalBoostSkillInstance(SignalBoostSkillData data) : base(data)
    {
        OnApply += tower =>
        {
            tower.RuntimeData.Range *= Data.IncreasedRange;
        };
        OnRemove += tower =>
        {
            tower.RuntimeData.Range /= Data.IncreasedRange;
        };
    }

    public override int SetPlayCount() => PlayCount;
}