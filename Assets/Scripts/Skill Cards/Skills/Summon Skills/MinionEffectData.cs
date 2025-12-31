using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Minion Effect", menuName = "StatusEffects/Minion")]
public class MinionEffectData : EffectData
{
    private void OnValidate()
    {
        description = $"An ally that can be summoned if there is at least one empty tile adjacent to the tower.";
    }
}
