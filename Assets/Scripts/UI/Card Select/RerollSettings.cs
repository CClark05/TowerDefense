using UnityEngine;

[CreateAssetMenu(fileName = "Reroll Settings", menuName = "CardSelect/RerollSettings")]
public class RerollSettings : ScriptableObject
{
    public int BaseCost = 25;
    public int IncreasePerRoll = 5;
    public int MaxRerolls = 1;
}