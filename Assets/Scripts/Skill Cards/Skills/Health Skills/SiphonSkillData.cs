using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Siphon Data", menuName = "SkillData/Health/Siphon")]
public class SiphonSkillData : SkillData
{
    public float HealthDropChance = 0.15f;

    private void OnValidate()
    {
        description = $"Enemies have a +{HealthDropChance * 100}% chance to drop 1 life on kill.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SiphonSkillInstance(this);
    }
}

public class SiphonSkillInstance : SkillInstance<SiphonSkillData>, IOnKill, IPlayCountPolicy<IOnKill>
{
    public SiphonSkillInstance(SiphonSkillData data) : base(data)
    {
    }


    public void OnKill(HitData hitData)
    {
        if (UnityEngine.Random.value < Data.HealthDropChance * PlayCount)
        {
            PlayerLife.Instance.AddLives(1);
            PlayCard();
        }
    }

    public int SetPlayCount() => 1;
}