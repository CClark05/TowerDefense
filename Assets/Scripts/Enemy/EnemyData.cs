using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int health;
    public float speed;
    public int livesCost;
    public int shields;
    public int coins;
    public GameObject prefab;
    public bool summonable = true;
    public bool IsBoss;
    public bool IsElite;

    public EliteBonus EliteBonus = new EliteBonus
    {
        healthMultiplier = 2f,
        speedMultiplier = 1.3f
    };
}

public struct EliteBonus
{
    public float healthMultiplier;
    public float speedMultiplier;
}