
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public abstract class CardSpawnable : MonoBehaviour
{
    [SerializeField] protected CardSpawnableEffectData effectData;
    public CardSpawnableEffectData EffectData => effectData;
    public CardSpawnableEffectData RuntimeData { get; protected set; }
    protected TowerDataHolder tower;
    public TowerDataHolder Tower => tower;
    public Action<int> OnDealtDamage;
    public virtual void Init(TowerDataHolder tower)
    {
        RuntimeData = EffectData.CloneRuntime();
        this.tower = tower;
        RuntimeData.obj = gameObject;
    }
}
public class Bomb : CardSpawnable
{
    [SerializeField] private GameObject aoeVisualPrefab, explosionSFXPrefab;
    [SerializeField] private AnimationClip bombIdle, bombFlash;
    
    private Coroutine explodeRoutine;
    private BombEffectData data;
    public override void Init(TowerDataHolder tower)
    {
        base.Init(tower);
        data = RuntimeData as BombEffectData;
        data.bomb = this;
        var rand = Random.Range(-1f, 1f);
        transform.position += new Vector3(rand, rand, 0);
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector2.zero;
        transform.DOScale(originalScale, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            GetComponent<SquishAnimation>().Squish(1, 1, () =>
            {
                GetComponent<IAnimationPlayer>().Play(bombIdle, transform);
            });
        });
        CallModifier.Call<IBombModifier>(tower.SkillContext, (mod, instance) =>
        {
            mod.Modify(RuntimeData as BombEffectData);
        });
        explodeRoutine = StartCoroutine(ExplodeRoutine());
        data.baseDamage = data.damage;
    }
    private IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds((RuntimeData as BombEffectData).timerDuration);
        Explode();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>() == null || data == null || explodeRoutine == null) return;
        Debug.Log("Explode Immediately");
        data.explodeOnShot.Invoke();
        StopCoroutine(explodeRoutine);
        explodeRoutine = null;
        StartCoroutine(Damage());
    }

    private void Explode()
    {
        var flash = GetComponent<IAnimationPlayer>().Play(bombFlash, transform);
        flash.OnComplete += () => StartCoroutine(Damage());
    }

    private IEnumerator Damage()
    {
        var visual = Instantiate(aoeVisualPrefab, transform.position, Quaternion.identity);
        float size = data.tileRadius * 4;
        visual.transform.localScale = new Vector3(size, size, size);
        visual.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).SetEase(Ease.InCubic).OnComplete(() => Destroy(visual));
        for (int i = 0; i < data.additionalExplosions + 1; i++)
        {
            Instantiate(explosionSFXPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            var enemies = Enumerable.ToHashSet(EnemyManager.Instance.GetNearbyEnemies(transform.position, data.tileRadius * 2).Select(e => e.GetComponent<IDamageable>()));
            foreach (var e in enemies)
            {
                var damageData = new HitData(data.damage, tower.GetComponent<TowerShooting>(), e, e.Transform.GetComponent<IUsesStatusEffects>());
                DamageService.ApplyDamage(damageData, e, tower.SkillContext);
                OnDealtDamage?.Invoke(data.damage);
            }
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }
}
