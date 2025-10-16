using UnityEngine;

public class PlayerGameOverStats : MonoBehaviour
{
    public static GameOverData GetGameOverData()
    {
        int enemiesKilled = 0;
        float maxDps = 0f;
        foreach (var tower in TowerDataHolder.ActiveTowerList)
        {
            enemiesKilled += tower.EnemiesKilled;
            if (tower.MaxWaveDPS > maxDps) maxDps = tower.MaxWaveDPS;
        }
        int highestWave = EnemyManager.Instance.CurrentWave - 1;
        int coins = PlayerInventory.Instance.TotalCoinsEarned;
        return new GameOverData(highestWave, enemiesKilled, maxDps, coins);
    }
}
