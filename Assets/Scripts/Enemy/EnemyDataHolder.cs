using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDataHolder : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    public void Init(EnemyData data)
    {
        this.data = data;
    }
}