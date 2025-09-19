using UnityEngine;
[CreateAssetMenu(fileName = "ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public string projectileName;
    public float speed;
    public int damage;
    public GameObject prefab;
}