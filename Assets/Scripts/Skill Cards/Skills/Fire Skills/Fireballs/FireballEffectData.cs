using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Fireball Effect", menuName = "StatusEffects/Fireball")]
public class FireballEffectData : CardSpawnableEffectData
{
    private void OnValidate()
    {
        description = $"Explodes on contact dealing {damage} base damage to the enemy.";
    }
    public Action<Vector2> OnExplode;
}

public abstract class CardSpawnableEffectData : EffectData 
{
    public int damage = 25;
    [HideInInspector] public int baseDamage;
    public GameObject prefab;
    [HideInInspector] public float damageMult = 1;
    [HideInInspector] public GameObject obj;
}