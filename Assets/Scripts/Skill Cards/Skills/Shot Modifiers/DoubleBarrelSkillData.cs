using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Double Barrel Data", menuName = "SkillData/Shot Modifiers/Double Barrel")]
public class DoubleBarrelSkillData : SkillData
{
    public float SpreadDegrees = 12;
    private void OnValidate()
    {
        description = "Fires 2 additional projectiles to the right and left of the original projectile.";
    }
    public override SkillInstance CreateInstance()
    {
        return new DoubleBarrelSkillInstance(this);
    }
}

public class DoubleBarrelSkillInstance : SkillInstance<DoubleBarrelSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public DoubleBarrelSkillInstance(DoubleBarrelSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
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
        yield return null;
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}
