using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Effect", menuName = "StatusEffects/Bomb")]
public class BombEffectData : CardSpawnableEffectData
{
    public float tileRadius = 1;
    public float timerDuration = 1;
    public Action explodeOnShot;
    [HideInInspector] public int additionalExplosions;
    public Bomb bomb;
    private void OnValidate()
    {
        description = $"After {timerDuration} second(s), deals {damage} base damage to all enemies within a {tileRadius} tile radius.";
    }
}
