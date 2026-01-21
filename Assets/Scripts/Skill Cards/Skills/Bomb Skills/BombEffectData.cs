using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Effect", menuName = "StatusEffects/Bomb")]
public class BombEffectData : EffectData
{
    public int damage = 25;
    [HideInInspector] public int baseDamage;
    public float tileRadius = 1;
    public float timerDuration = 1;
    public Action explodeOnShot;
    public GameObject bombPrefab;
    [HideInInspector] public float damageMult = 1;
    [HideInInspector] public int additionalExplosions;
    public Bomb bomb;
    private void OnValidate()
    {
        description = $"After {timerDuration} second(s), deals {damage} damage to all enemies within a {tileRadius} tile radius.";
    }
}
