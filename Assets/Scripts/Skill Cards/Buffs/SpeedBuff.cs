using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Speed Buff", menuName = "Buffs/Speed")]
public class SpeedBuffData : BuffData, IBuff
{
    public float speedIncrease = 1.25f;
    private void OnValidate()
    {
        description = $"Increases fire rate by {(speedIncrease - 1) * 100}% per stack.";
    }
    
    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSpeed *= speedIncrease;
    }
    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSpeed /= speedIncrease;
    }

    
}