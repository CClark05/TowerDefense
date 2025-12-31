using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Black Hole Data", menuName = "SkillData/Generic/Black Hole")]
public class BlackHoleSkillData : SkillData
{
    public float tileSize = 0.5f;
    public GameObject BlackHoleVisualPrefab;
    public float duration = 0.5f;
    private void OnValidate()
    {
        description = $"On kill create a black hole that pulls in any enemies within a +{tileSize} tile radius.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BlackHoleSkillInstance(this);
    }
}

public class BlackHoleSkillInstance : SkillInstance<BlackHoleSkillData>, IOnKill, IPlayCountPolicy<IOnKill>
{
    public BlackHoleSkillInstance(BlackHoleSkillData data) : base(data)
    {
    }

    public void OnKill(HitData hitData)
    {
        GameObject visual = Object.Instantiate(Data.BlackHoleVisualPrefab, hitData.damageable.Transform.position, Quaternion.identity);
        float size = Data.tileSize * 4 * PlayCount;
        visual.transform.localScale = new Vector3(size, size, size);
        var position = hitData.damageable.Transform.position;
        var enemies = EnemyManager.Instance.GetNearbyEnemies(position, Data.tileSize * 2 * PlayCount);
        visual.GetComponent<SpriteRenderer>().DOFade(0, Data.duration).OnComplete(() => Object.Destroy(visual));
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<IMovementOverride>().MoveTo(position, Data.duration);
        }
    }

    public int SetPlayCount() => 1;
}