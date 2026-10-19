using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Wave Data", menuName = "Waves/Wave Data")]
public class WaveData : ScriptableObject
{
    [FormerlySerializedAs("enemies")] public WaveEntry[] entries;
}

[System.Serializable]
public class WaveEntry
{
    public EnemyCost enemy;
    public bool isElite;
    public EnemyCost[] enemiesUnderneath;
    public int count;
    public float delay;
}