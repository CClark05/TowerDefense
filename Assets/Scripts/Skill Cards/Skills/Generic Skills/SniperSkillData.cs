using UnityEngine;

[CreateAssetMenu(fileName = "Sniper Data", menuName = "SkillData/Generic/Sniper")]
public class SniperSkillData : SkillData
{
    public int plusDamage = 3;
    public int tileInterval = 2;

    private void OnValidate()
    {
        description = $"Gain +{plusDamage} base damage per {tileInterval} tiles between you and the target.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SniperSkillInstance(this);
    }
}

public class SniperSkillInstance : SkillInstance<SniperSkillData>, IOnHit
{
    public SniperSkillInstance(SniperSkillData data) : base(data)
    {
    }

    public void OnHit(HitData hitData)
    {
        PlayCard();
        int tiles = Mathf.CeilToInt(Vector2.Distance(hitData.damageable.Transform.position, skillContext.Tower.transform.position) / GridManager.Instance.CellSize);
        hitData.finalDamage += (tiles / Data.tileInterval) * Data.plusDamage;
    }
}