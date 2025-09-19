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
        randomCard = closestTower.SkillDataList[UnityEngine.Random.Range(0, closestTower.SkillDataList.Count)];
        towerWaveData.addedCards.Add(randomCard);
        closestTower.TryRemoveCard(randomCard);
        skillContext.OnCardInstanceCreated += OnInstanceAdded;
        void OnInstanceAdded(SkillInstance instance)
        {
            if (instance.Data != randomCard) return;
            instance.PlayTwice = true;
            skillContext.OnCardInstanceCreated -= OnInstanceAdded;
        }
        PlayCard();
    }

    void ITowerWaveEndModifier.Modify(TowerWaveData towerWaveData)
    {
        if (closestTower == null) return;
        closestTower.AddCard(randomCard);
        towerWaveData.removedCards.Add(randomCard);
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
    
}
