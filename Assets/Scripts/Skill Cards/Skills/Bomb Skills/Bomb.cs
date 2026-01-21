
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bomb : MonoBehaviour
{
    [SerializeField] private BombEffectData bombEffectData;
    public BombEffectData BombEffectData => bombEffectData;
    [SerializeField] private GameObject aoeVisualPrefab, explosionSFXPrefab;
    [SerializeField] private AnimationClip bombIdle, bombFlash;
    public BombEffectData RuntimeData { get; private set; }
    private TowerDataHolder tower;
    public TowerDataHolder Tower => tower;
    public event Action<int> OnDealtDamage;
    private Coroutine explodeRoutine;
    public static event Action<Bomb, int> OnDealtDamageStatic;
    public void Init(TowerDataHolder tower)
    {
        RuntimeData = bombEffectData.CloneRuntime();
        RuntimeData.bomb = this;
        this.tower = tower;
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
            mod.Modify(RuntimeData);
        });
        explodeRoutine = StartCoroutine(ExplodeRoutine());
        RuntimeData.baseDamage = RuntimeData.damage;
    }
    private IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(RuntimeData.timerDuration);
        Explode();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Projectile>() == null || RuntimeData.explodeOnShot == null || explodeRoutine == null) return;
        Debug.Log("Explode Immediately");
        RuntimeData.explodeOnShot.Invoke();
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
        float size = RuntimeData.tileRadius * 4;
        visual.transform.localScale = new Vector3(size, size, size);
        visual.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).SetEase(Ease.InCubic).OnComplete(() => Destroy(visual));
        for (int i = 0; i < RuntimeData.additionalExplosions + 1; i++)
        {
            Instantiate(explosionSFXPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            var enemies = EnemyManager.Instance.GetNearbyEnemies(transform.position, RuntimeData.tileRadius * 2).Select(e => e.GetComponent<IDamageable>()).ToHashSet();
            foreach (var e in enemies)
            {
                var damageData = new HitData(RuntimeData.damage, tower.GetComponent<TowerShooting>(), e, e.Transform.GetComponent<IUsesStatusEffects>());
                DamageService.ApplyDamage(damageData, e, tower.SkillContext);
                OnDealtDamage?.Invoke(RuntimeData.damage);
                OnDealtDamageStatic?.Invoke(this, RuntimeData.damage);
            }
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }
}
