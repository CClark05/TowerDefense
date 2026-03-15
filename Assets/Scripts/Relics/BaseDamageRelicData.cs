using UnityEngine;

[CreateAssetMenu(fileName = "Base Damage Relic Data", menuName = "Relics/Base Damage")]
public class BaseDamageRelicData : RelicData
{
    public int BonusDamage;
    private void OnValidate()
    {
        description = $"All towers gain +{BonusDamage} base damage.";
    }
    public override RelicInstance CreateInstance()
    {
        return new BaseDamageRelicInstance(this);
    }
}
public class BaseDamageRelicInstance : RelicInstance
{
    public BaseDamageRelicInstance(BaseDamageRelicData data) : base(data)
    {
        BuildingManager.Instance.OnAddNewTower += OnAddNewTower;
    }
    void OnAddNewTower(TowerDataHolder tower)
    {
        tower.RuntimeData.BaseDamage += ((BaseDamageRelicData)Data).BonusDamage;
    }
    public override void OnPickup()
    {
        foreach(var tower in TowerDataHolder.ActiveTowerList)
        {
            tower.RuntimeData.BaseDamage += ((BaseDamageRelicData)Data).BonusDamage;
        }
    }
}