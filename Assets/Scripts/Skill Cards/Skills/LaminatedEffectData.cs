using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Laminated Effect", menuName = "StatusEffects/Laminated")]
public class LaminatedEffectData : EffectData
{
    private void OnValidate()
    {
        description = "Card cannot be upgraded.";
    }
}
