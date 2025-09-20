using System.Collections.Generic;
using UnityEngine;

public class TowerWaveData
{
    public List<SkillData> addedCards = new();
    public List<SkillData> removedCards = new();
    public float increasedRange;
    public int increasedSlots;
    public float increasedSpeed = 1;
    public float stunnedDuration;
}

