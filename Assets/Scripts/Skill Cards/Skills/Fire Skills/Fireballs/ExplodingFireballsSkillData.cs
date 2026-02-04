using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Exploding Fireball Data", menuName = "SkillData/Fire/Exploding Fireball")]
public class ExplodingFireballsSkillData : SkillData
{
    public float Radius = 1;
    public GameObject fireCirclePrefab;
    public float duration = 2f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        string hex2 = ColorUtility.ToHtmlStringRGB(statusEffects[1].data.color);
        description = $"When your <color=#{hex}>Fireballs</color> explode they create a {Radius} tile radius circle of fire that applies +1 <color=#{hex2}>Fire</color> per second to all enemies inside. Circles lasts {duration} seconds.";
    }

    public override SkillInstance CreateInstance()
    {
        return new ExplodingFireballsSkillInstance(this);
    }
}
public class ExplodingFireballsSkillInstance : SkillInstance<ExplodingFireballsSkillData>, IFireballModifier, IPlayCountPolicy<IFireballModifier>
{
    public ExplodingFireballsSkillInstance(ExplodingFireballsSkillData data) : base(data)
    {
    }
    public void Modify(FireballEffectData fireballData)
    {
        fireballData.OnExplode += (position) =>
        {
            var fireCircle = UnityEngine.Object.Instantiate(Data.fireCirclePrefab, position, Quaternion.identity).GetComponent<FireCircle>();
            fireCircle.Init(Data.Radius, Data.duration, skillContext);
        };
    }
    public int SetPlayCount() => 1;
    
}