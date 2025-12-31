using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Arrow Effect", menuName = "StatusEffects/Arrow")]
public class ArrowEffectData : EffectData
{
    public ProjectileData projectileData;

    private void OnValidate()
    {
        description = $"Deals +{projectileData.damage} damage on hit.";
    }
}
