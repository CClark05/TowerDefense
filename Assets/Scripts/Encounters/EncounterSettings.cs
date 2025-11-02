using UnityEngine;

[CreateAssetMenu(fileName = "EncounterSettings", menuName = "Settings/EncounterSettings")]
public class EncounterSettings : ScriptableObject
{
    public float targetRate = 0.15f;
    public int firstWave;
    public Sprite icon;
}