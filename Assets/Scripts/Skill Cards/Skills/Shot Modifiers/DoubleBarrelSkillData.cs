using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Double Barrel Data", menuName = "SkillData/Shot Modifiers/Double Barrel")]
public class DoubleBarrelSkillData : SkillData
{
    public float SpreadDegrees = 12;
    public float fireRateReduction = 0.25f;
    private void OnValidate()
    {
        description = $"Fires 2 additional projectiles to the right and left of the original projectile. Reduces fire rate by {fireRateReduction * 100}%.";
    }
    public override SkillInstance CreateInstance()
    {
        return new DoubleBarrelSkillInstance(this);
    }
}

public class DoubleBarrelSkillInstance : SkillInstance<DoubleBarrelSkillData>, IShotModifier, IPlayCountPolicy<IShotModifier>, ITowerCardReceivedModifier
{
    public DoubleBarrelSkillInstance(DoubleBarrelSkillData data) : base(data)
    {
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSpeed *= 1f - Data.fireRateReduction;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSpeed /= 1 - Data.fireRateReduction;
    }
    public void Modify(ProjectileShotData shotData)
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
    }
    
    public int SetPlayCount() => 1;
    
}
