using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Bandit's Secret Data", menuName = "SkillData/Generic/BanditsSecret")]
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
    public BanditsSecretSkillInstance(BanditsSecretSkillData data) : base(data)
    {
        
    }
    void ITowerWaveStartModifier.Modify(TowerWaveData towerWaveData)
    {
        closestTower = FindClosestTower();
        if (closestTower == null) return;
        var pool = closestTower.WaveData.startingSnapshot;
        if (pool.Count == 0) return;
        randomCard = pool[UnityEngine.Random.Range(0, pool.Count)];
        towerWaveData.borrowRequests.Add(new BorrowRequest
        {
            borrower = towerWaveData.owner,
            lender = closestTower,
            card = randomCard
        });
        skillContext.OnCardInstanceCreated += OnInstanceAdded;
        void OnInstanceAdded(SkillInstance instance)
        {
            if (instance.Data != randomCard) return;
            instance.PlayTwice = true;
            if (instance.Data == Data) return;
            skillContext.OnCardInstanceCreated -= OnInstanceAdded;
        }
        PlayCard();
    }
    void ITowerWaveEndModifier.Modify(TowerWaveData towerWaveData)
    {
        closestTower = FindClosestTower();
        if (closestTower == null) return;
        /**
        Debug.Log("test");
        closestTower.AddCard(randomCard);
        towerWaveData.removedCards.Add(randomCard);
        */
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

    public override void Dispose()
    {
        base.Dispose();
        if (skillContext?.OnCardInstanceCreated != null)
            skillContext.OnCardInstanceCreated = null;
    }
}
