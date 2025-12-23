using System.Collections.Generic;
using UnityEngine;

public class TowerWaveData
{
    public TowerDataHolder owner;
    public List<SkillInstance> addedCards = new();
    public List<SkillInstance> removedCards = new();
    public List<BorrowRequest> borrowRequests = new();
    public List<SkillInstance> startingSnapshot = new();
    public float increasedRange = 1;
    public int increasedSlots;
    public int increasedBaseDamage;
    public float increasedSpeed = 1;
    public float stunnedDuration;
}