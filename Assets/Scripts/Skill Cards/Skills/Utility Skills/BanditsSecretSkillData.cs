using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Bandit's Secret Data", menuName = "SkillData/Utility/BanditsSecret")]
public class BanditsSecretSkillData : SkillData
{
    public override SkillInstance CreateInstance()
    {
        return new BanditsSecretSkillInstance(this);
    }
}

public class BanditsSecretSkillInstance : SkillInstance<BanditsSecretSkillData>, IPreWaveStartModifier, ITowerWaveEndModifier, ITowerCardReceivedModifier, IPlayCountPolicy<IPreWaveStartModifier>
{
    private SkillInstance randomCard;
    private TowerDataHolder closestTower;
    private List<SkillInstance> stolenCards = new();
    public BanditsSecretSkillInstance(BanditsSecretSkillData data) : base(data)
    {
        
    }
    
    void IPreWaveStartModifier.Modify(TowerWaveData towerWaveData)
    {
        closestTower = FindClosestTower();
        if (closestTower == null) return;
        var pool = closestTower.WaveData.startingSnapshot.Where(card => !stolenCards.Contains(card) && !card.Laminated).ToList();
        if (pool.Count == 0) return;
        randomCard = pool[UnityEngine.Random.Range(0, pool.Count)];
        stolenCards.Add(randomCard);
        var request = new BorrowRequest(towerWaveData.owner, closestTower, randomCard);
        request.playCount++;
        towerWaveData.borrowRequests.Add(request);
        PlayCard();
    }
    private TowerDataHolder FindClosestTower()
    {
        (TowerDataHolder tower, float distance) closestTower = (null, Mathf.Infinity);
        foreach (var tower in TowerDataHolder.ActiveTowerList)
        {
            if(tower == skillContext.Tower) continue;
            float distance = Vector2.Distance(tower.transform.position, skillContext.Tower.transform.position);
            if (distance < closestTower.distance)
            {
                closestTower.tower = tower;
                closestTower.distance = distance;
            }
        }
        return closestTower.tower;
    }

    public void Modify(TowerWaveData towerWaveData)
    {
        stolenCards.Clear();
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSlots++;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSlots--;
    }

    public int SetPlayCount()
    {
        return 1;
    }
}
