using UnityEngine;
[CreateAssetMenu(fileName = "Longer Bombs Data", menuName = "SkillData/Bomb/Longer Bombs")]
public class LongerBombsSkillData : SkillData
{
    public float DurationIncrease = 2.0f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Your <color=#{hex}>Bombs</color> last +{DurationIncrease} second(s) longer before exploding, however they explode an additional time.";
    }
    public override SkillInstance CreateInstance()
    {
        return new LongerBombsSkillInstance(this);
    }
}

public class LongerBombsSkillInstance : SkillInstance<LongerBombsSkillData>, IBombModifier, IPlayCountPolicy<IBombModifier>
{
    public LongerBombsSkillInstance(LongerBombsSkillData data) : base(data)
    {
    }

    public void Modify(BombEffectData bombData)
    {
        bombData.timerDuration += Data.DurationIncrease;
        bombData.additionalExplosions += PlayCount;
    }

    public int SetPlayCount() => 1;
}
