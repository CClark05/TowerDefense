using UnityEngine;
[CreateAssetMenu(fileName = "EnemyCost", menuName = "Enemy/EnemyCost")]
public class EnemyCost : ScriptableObject
{
    public EnemyData EnemyData;
    public int MinWave = 1;
    //public float Weight = 1;
}