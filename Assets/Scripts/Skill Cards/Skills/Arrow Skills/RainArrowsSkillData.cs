using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Rain Arrows Data", menuName = "SkillData/Arrow/Rain Arrows")]
public class RainArrowsSkillData : SkillData
{
    public int KillsNeeded = 3;
    public int Arrows = 5;
    public GameObject VisualPrefab;
    public float TileSize = 1;
    public float duration = 2f;
    public GameObject ArrowPrefab;
    public ProjectileData ArrowData;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Every {KillsNeeded} kills rain down +{Arrows} <color=#{hex}>Arrows</color> in a {TileSize} tile radius.";
    }

    public override SkillInstance CreateInstance()
    {
        return new RainArrowsSkillInstance(this);
    }
}

public class RainArrowsSkillInstance : SkillInstance<RainArrowsSkillData>, IOnKill, IPlayCountPolicy<IOnKill>
{
    private int killCounter;

    public RainArrowsSkillInstance(RainArrowsSkillData data) : base(data)
    {
        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
    }

    private void OnWaveComplete() => killCounter = 0;

    public void OnKill(HitData hitData)
    {
        killCounter++;
        if (killCounter % Data.KillsNeeded != 0) return;
        var position = hitData.damageable.Transform.position;
        GameObject visual = Object.Instantiate(Data.VisualPrefab, position, Quaternion.identity);
        float size = Data.TileSize * 4;
        visual.transform.localScale = new Vector3(size, size, size);
        visual.GetComponent<SpriteRenderer>().DOFade(0, Data.duration).SetEase(Ease.InCubic).OnComplete(() => Object.Destroy(visual));
        CoroutineRunner.Instance.StartCoroutine(SpawnArrows());
        IEnumerator SpawnArrows()
        {
            float delayBetweenArrows = (Data.duration / (Data.Arrows * PlayCount)) * 0.5f;
            float arrowDuration = 0.5f;
            for (int i = 0; i < Data.Arrows * PlayCount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * (Data.TileSize * 2);
                Vector3 spawnPosition = position + new Vector3(randomOffset.x, randomOffset.y, 0);
                GameObject arrow = Object.Instantiate(Data.ArrowPrefab, spawnPosition, Quaternion.identity);
                float yPos = arrow.transform.position.y;
                arrow.transform.position = new Vector2(arrow.transform.position.x, 14f);
                arrow.GetComponent<BoxCollider2D>().enabled = false;
                arrow.transform.localScale = new Vector2(arrow.transform.localScale.x, -arrow.transform.localScale.y);
                Sequence arrowAnimation = DOTween.Sequence();
                arrowAnimation.Append(arrow.transform.DOMoveY(yPos, 0.5f).SetEase(Ease.InCubic).OnComplete(() => arrow.GetComponent<BoxCollider2D>().enabled = true));
                arrowAnimation.Append(arrow.GetComponent<SpriteRenderer>().DOFade(0, arrowDuration).SetEase(Ease.InCubic).OnComplete(() => Object.Destroy(arrow)));
                arrow.GetComponent<Arrow>().OnHit += damageable =>
                {
                    var damageData = new HitData(Data.ArrowData.damage, hitData.tower, damageable, damageable.Transform.GetComponent<IUsesStatusEffects>());
                    DamageService.ApplyDamage(
                        damageData,
                        damageable,
                        damageable.Transform.GetComponent<IUsesStatusEffects>(),
                        skillContext,
                        (_hitData, pos) => {_hitData.tower.DealtDamage(_hitData,pos);});
                };
                yield return new WaitForSeconds(delayBetweenArrows);
            }
        }
    }

    public int SetPlayCount() => 1;
    public override void Dispose()
    {
        base.Dispose();
        EnemyManager.Instance.OnWaveComplete -= OnWaveComplete;
    }
}