using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Double Barrel Data", menuName = "SkillData/Tower Modifiers/Double Barrel")]
public class DoubleBarrelSkillData : SkillData
{
    public float SpreadDegrees = 12;
    public float bulletLifetime = 1;
    private void OnValidate()
    {
        description = $"Tower fires a spread of 3 close-range shotgun pellets.";
    }
    public override SkillInstance CreateInstance()
    {
        return new DoubleBarrelSkillInstance(this);
    }
}

public class DoubleBarrelSkillInstance : SkillInstance<DoubleBarrelSkillData>, IShotModifier, IPlayCountPolicy<IShotModifier>
{
    public DoubleBarrelSkillInstance(DoubleBarrelSkillData data) : base(data)
    {
    }
    public void Modify(ProjectileShotData shotData)
    {
        shotData.lifetimeDuration = Data.bulletLifetime;
        Vector2 forward = shotData.originalDirection;
        Vector2 left  = Rotate(forward, Data.SpreadDegrees);
        Vector2 right = Rotate(forward, -Data.SpreadDegrees);
        shotData.directionOverrides.Add(left);
        shotData.directionOverrides.Add(right);
        Vector2 Rotate(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            return new Vector2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            ).normalized;
        }
    }
    
    public int SetPlayCount() => 1;
    
}
