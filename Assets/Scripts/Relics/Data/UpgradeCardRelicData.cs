using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Card Relic Data", menuName = "Relics/Upgrade Cards")]
public class UpgradeCardRelicData : RelicData
{
    public float PercentChance = 0.15f;
    private void OnValidate()
    {
        description = $"When you add a new card to your hand it has a {PercentChance * 100}% chance to upgrade one level.";
    }

    public override RelicInstance CreateInstance()
    {
        return new UpgradeCardRelicInstance(this);
    }
}
public class UpgradeCardRelicInstance : RelicInstance
{
    public UpgradeCardRelicInstance(UpgradeCardRelicData data) : base(data)
    {
        
    }
    public override void OnPickup()
    {
        InventoryUI.Instance.OnAddNewCard += instance =>
        {
            Debug.Log("new card");
            if (UnityEngine.Random.value < ((UpgradeCardRelicData)Data).PercentChance)
            {
                instance.PlayCount++;
                OnUsed?.Invoke();
            }
        };
    }
}