using System;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class TowerRangeCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        var damageable = other.GetComponent<IDamageable>();
        var skillContext = GetComponentInParent<TowerDataHolder>().SkillContext;
        CallModifier.Call<IOnEnemyEnteredRange>(skillContext, (mod,instance) => mod.OnEnemyEnteredRange(damageable));
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        var damageable = other.GetComponent<IDamageable>();
        var skillContext = GetComponentInParent<TowerDataHolder>().SkillContext;
        CallModifier.Call<IOnEnemyEnteredRange>(skillContext, (mod,instance) => mod.OnEnemyLeftRange(damageable));
    }
}
