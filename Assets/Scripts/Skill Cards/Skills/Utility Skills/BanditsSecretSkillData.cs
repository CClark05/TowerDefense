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

public class BanditsSecretSkillInstance : SkillInstance<BanditsSecretSkillData>, ITowerWaveStartModifier, ITowerWaveEndModifier
{
    private SkillData randomCard;
    private TowerDataHolder closestTower;
    private List<SkillData> stolenCards = new();
    public BanditsSecretSkillInstance(BanditsSecretSkillData data) : base(data)
    {
        
    }
    void ITowerWaveStartModifier.Modify(TowerWaveData towerWaveData)
    {
        closestTower = FindClosestTower();
        if (closestTower == null) return;
        var pool = closestTower.WaveData.startingSnapshot.Where(card => !stolenCards.Contains(card.Data)).ToList();
        if (pool.Count == 0) return;
        randomCard = pool[UnityEngine.Random.Range(0, pool.Count)].Data;
        stolenCards.Add(randomCard);
        var request = new BorrowRequest
        {
            borrower = towerWaveData.owner,
            lender = closestTower,
            card = randomCard,
        };
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
}
