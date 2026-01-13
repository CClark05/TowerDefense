using System;
using UnityEngine;

public class EnemyScaling : MonoBehaviour
{
    public int MaxHP { get; private set; }
    public float SpeedIncrease { get; private set; }
    private void Awake()
    {
        int cycle = (EnemyManager.Instance.CurrentWave- 1) / EncounterGenerator.Instance.CycleLength;
        float mult = 1f + cycle * EnemyManager.Instance.WaveSettings.HpIncreaseAfterBoss;
        MaxHP = Mathf.CeilToInt(GetComponent<EnemyDataHolder>().Data.health * mult);
        SpeedIncrease = cycle * EnemyManager.Instance.WaveSettings.SpeedIncreaseAfterBoss;
        
    }
}
