using UnityEngine;

[CreateAssetMenu(fileName = "EncounterSettings", menuName = "Settings/EncounterSettings")]
public class EncounterSettings : ScriptableObject
{
    public int showEveryXWaves = 4;
    public int firstWave;
}