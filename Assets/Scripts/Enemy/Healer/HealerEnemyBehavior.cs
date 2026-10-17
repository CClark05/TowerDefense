using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class HealerEnemyBehavior : MonoBehaviour
{
    [SerializeField] private int healAmount = 10;
    [SerializeField] private float healRadius = 5f;
    [SerializeField] private float healCooldown = 3f;
    [SerializeField] private SpriteRenderer healCircle;
    private float healTimer;
    private List<EnemyHealth> currentEnemies => EnemyManager.Instance.CurrentEnemies
            .Select(e => e.GetComponent<EnemyHealth>())
            .ToList();

    private void Start()
    {
        healCircle.transform.localScale = new Vector3(healRadius * 2, healRadius * 2, 1);
        healCircle.color = new Color(healCircle.color.r, healCircle.color.g, healCircle.color.b, 0f);
    }

    private void Update()
    {
        healTimer += Time.deltaTime;
        if (healTimer > healCooldown)
        {
            Heal();
        }
    }
    
    private void Heal()
    {
        bool healedAny = false;
        foreach (var enemy in currentEnemies)
        {
            float sqrDist = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDist <= healRadius * healRadius)
            {
                if(enemy.HealthSystem.Health >= enemy.HealthSystem.MaxHealth || enemy.gameObject == gameObject) continue;
                enemy.Heal(healAmount);
                Debug.Log("HEAL");
                healedAny = true;
            }
        }
        if (healedAny)
        {
            HealAnimation();
            healTimer = 0f;
        }
    }

    private void HealAnimation()
    {
        float duration = 0.5f;
        healCircle.DOFade(0.4f, duration).OnComplete(() =>
        {
            healCircle.DOFade(0f, duration).SetDelay(0.25f);
        });
    }
}
