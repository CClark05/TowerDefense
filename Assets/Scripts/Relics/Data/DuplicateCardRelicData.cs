using UnityEngine;

[CreateAssetMenu(fileName = "Duplicate Card Relic Data", menuName = "Relics/Duplicate Cards")]
public class DuplicateCardRelicData : RelicData
{
    public float PercentChance = 0.15f;
    private void OnValidate()
    {
        description = $"When you add a new card to your hand it has a {PercentChance * 100}% chance to duplicate.";
    }
    public override RelicInstance CreateInstance()
    {
        return new DuplicateCardRelicInstance(this);
    }
}
public class DuplicateCardRelicInstance : RelicInstance
{
    public DuplicateCardRelicInstance(DuplicateCardRelicData data) : base(data)
    {
        
    }
    public override void OnPickup()
    {
        InventoryUI.Instance.OnAddNewCard += instance =>
        {
            if(UnityEngine.Random.value < ((DuplicateCardRelicData)Data).PercentChance)
            {
                var newInstance = instance.Data.CreateInstance();
                newInstance.PlayCount = instance.PlayCount;
                InventoryUI.Instance.AddCard(newInstance);
                OnUsed?.Invoke();
            }
            
        };
    }
}