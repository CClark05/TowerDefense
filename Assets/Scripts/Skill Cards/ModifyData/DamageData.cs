using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    public int finalDamage;
    public List<Color> colors = new();
    public float damageMarkerSizeMult = 1;
    public bool damageMarkerPunchEffect;
    public bool didKill;
}