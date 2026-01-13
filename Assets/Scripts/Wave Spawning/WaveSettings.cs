using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "Waves/WaveSettings")]
public class WaveSettings : ScriptableObject
{
    [Header("Budget Curve")]
    public float BaseBudget = 30f;     // wave 1
    public float GrowthRate = 1.15f;   // multiplicative
    public float AdditivePerWave = 1.5f;
    public float Variance = 0.08f;     // ±8% noise

    [Header("Cadence")]
    public float SpikeAfterBoss = 1.4f;

    [Header("Enemy Pool")]
    public List<EnemyCost> Enemies;
    public float HpIncreaseAfterBoss = 0.25f;
    public float SpeedIncreaseAfterBoss = 0.1f;
    public int minEnemies = 4;
}
