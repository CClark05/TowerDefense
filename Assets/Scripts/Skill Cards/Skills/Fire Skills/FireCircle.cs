using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class FireCircle : MonoBehaviour
{
    private List<IDamageable> enemiesInCircle = new();
    [SerializeField] private FireStatusEffect fireStatusEffect;
    private SkillContext skillContext;
    public void Init(float tileRadius, float duration, SkillContext skillContext)
    {
        float size = tileRadius * 4;
        transform.localScale = new Vector3(size, size, size);
        GetComponent<SpriteRenderer>().DOFade(0, duration).SetEase(Ease.InCubic).OnComplete(() => Destroy(gameObject));
        this.skillContext = skillContext;
        StartCoroutine(ApplyFire());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        var enemy = other.GetComponent<IDamageable>();
        enemiesInCircle.Add(enemy);
        
    }
    private IEnumerator ApplyFire()
    {
        yield return new WaitUntil(() => enemiesInCircle.Count > 0);
        while (true)
        {
            foreach (var enemy in enemiesInCircle)
            {
                var damageData = new HitData(1, skillContext.Tower.GetComponent<TowerShooting>(), enemy, enemy.Transform.GetComponent<IUsesStatusEffects>());
                PersistentStatusEffect.AddPersistentEffect(fireStatusEffect, skillContext, enemy.Transform.GetComponent<IUsesStatusEffects>(), damageData, 1);
                Debug.Log("applied fire");
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        var enemy = other.GetComponent<IDamageable>();
        if (!enemiesInCircle.Contains(enemy))
        {
            Debug.LogError("SOMETHING WENT WRONG!!!");
            return;
        }
        enemiesInCircle.Remove(enemy);
    }
}
