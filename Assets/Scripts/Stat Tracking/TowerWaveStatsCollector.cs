using System;
using System.Linq;
using UnityEngine;

public class TowerWaveStatsCollector : MonoBehaviour
{
    private IWaveStatsSink sink;
    private void Start()
    {
        sink = new TowerStatsCSV();
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            var towers = TowerDataHolder.ActiveTowerList.Cast<ITowerStatsProvider>();
            sink.AppendTowerRows(RunData.RunId, EnemyManager.Instance.CurrentWave - 1, towers);
            Debug.Log("wrote to csv");
        };
    }
    [ContextMenu("Clear CSV data")]
    public void ClearCSV() => sink.ClearAllEntries();
}
