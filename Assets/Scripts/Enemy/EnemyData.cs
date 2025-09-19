using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public int health;
    public float speed;
    public int livesCost;
    public int shields;
    public GameObject prefab;
}