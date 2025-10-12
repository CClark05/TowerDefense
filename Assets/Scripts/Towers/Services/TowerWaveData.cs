using System.Collections.Generic;
using UnityEngine;

public class TowerWaveData
{
    public TowerDataHolder owner;
    public List<SkillData> addedCards = new();
    public List<SkillData> removedCards = new();
    public List<BorrowRequest> borrowRequests = new();
    public List<SkillInstance> startingSnapshot = new();
    public float increasedRange;
    public int increasedSlots;
    public float increasedSpeed = 1;
    public float stunnedDuration;
}