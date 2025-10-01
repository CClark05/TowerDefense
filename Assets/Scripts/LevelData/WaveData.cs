using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WaveData")]
public class WaveData : ScriptableObject
{
    public List<EnemyWaveData> enemies;
    public float delayBetweenSpawns;
    public int movesAllowed;
}
[System.Serializable]
public class EnemyWaveData
{
    public EnemyData data;
    public int count = 1;
    public float delay;
}