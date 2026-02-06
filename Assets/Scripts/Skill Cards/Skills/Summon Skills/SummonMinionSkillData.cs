using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "Summon Minion Data", menuName = "SkillData/Summon/Summon Minion")]
public class SummonMinionSkillData : SkillData
{
    public GameObject minionPrefab;
    public ProjectileData projectileData;
    public float range;
    public float timeBetweenShots;
    public GridObjectData towerGridData;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        string hex2 = ColorUtility.ToHtmlStringRGB(statusEffects[1].data.color);
        description =
            $"Summon +1 <color=#{hex}>Minion(s)</color>. Each <color=#{hex}>Minion</color> shoots an <color=#{hex2}>Arrow</color> every {timeBetweenShots} second at enemies within their range.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SummonMinionSkillInstance(this);
    }
}

public class SummonMinionSkillInstance : SkillInstance<SummonMinionSkillData>, ITowerWaveStartModifier, ITowerWaveEndModifier
{
    private Vector2? minionPosition;
    private List<IMinion> summons = new();

    public SummonMinionSkillInstance(SummonMinionSkillData data) : base(data)
    {
    }

    void ITowerWaveStartModifier.Modify(TowerWaveData towerWaveData)
    {
        if (minionPosition == null)
        {
            var position = towerWaveData.owner.transform.position;
            var possiblePositions = new[]
            {
                new Vector2(position.x + 2, position.y),
                new Vector2(position.x - 2, position.y),
                new Vector2(position.x, position.y + 2),
                new Vector2(position.x, position.y - 2)
            }.OrderBy(_ => UnityEngine.Random.value).ToArray();

            foreach (var pos in possiblePositions)
            {
                GridManager.Instance.Grid.GetXY(pos, out int x, out int y);
                if (GridManager.Instance.Grid.GetValue(x, y).id == 0)
                {
                    minionPosition = pos;
                    GridManager.Instance.Grid.SetValue(x, y, Data.towerGridData);
                    break;
                }
            }

            if (minionPosition == null) return;
        }

        var minion = Object.Instantiate(Data.minionPrefab, towerWaveData.owner.transform).GetComponent<IMinion>();
        float tileSize = 2;
        float step = tileSize * 0.18f;
        float maxRadius = tileSize * 0.2f;
        Vector2 offset = GetSpiralOffset(summons.Count, step, maxRadius);
        minion.Transform.position = (minionPosition.Value + offset);
        minion.Init(Data.projectileData, Data.range, Data.timeBetweenShots);
        minion.OnHit += (damageable, damage) =>
        {
            var damageData = new HitData(damage, towerWaveData.owner.GetComponent<TowerShooting>(), damageable, damageable.Transform.GetComponent<IUsesStatusEffects>());
            DamageService.ApplyDamage(
                damageData,
                damageable,
                damageable.Transform.GetComponent<IUsesStatusEffects>(),
                skillContext,
                (_hitData, pos) =>
                {
                    _hitData.tower.AddHit();
                    _hitData.tower.DealtDamage(_hitData, pos);
                    Damage += damage;
                });
        };
        summons.Add(minion);
    }

    void ITowerWaveEndModifier.Modify(TowerWaveData towerWaveData)
    {
        if (minionPosition.HasValue)
        {
            GridManager.Instance.Grid.GetXY(minionPosition.Value, out int x, out int y);
            GridManager.Instance.SetEmpty(x, y);
            minionPosition = null;
        }
        foreach (var s in summons)
            s.Destroy();
        summons.Clear();
    }


    private Vector2 GetSpiralOffset(int index, float step, float maxRadius)
    {
        if (index == 0) return Vector2.zero;
        // index = 0 -> near center, 1 -> slightly out, etc.
        const float goldenAngle = 2.39996323f; // radians (~137.5 degrees)

        float r = Mathf.Min(step * (index + 1), maxRadius);
        float a = index * goldenAngle;

        return new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
    }
}