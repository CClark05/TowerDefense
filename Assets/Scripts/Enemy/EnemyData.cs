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
}