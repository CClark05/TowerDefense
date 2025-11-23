using UnityEngine;

public class EnemyDataHolder : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;
    public void Init(EnemyData data)
    {
        this.data = data;
        if (data.IsElite)
        {
            this.data = data.CloneRuntime();
            this.data.health = Mathf.CeilToInt(this.data.health * data.EliteBonus.healthMultiplier);
            this.data.speed *= data.EliteBonus.speedMultiplier;
        }
    }
}