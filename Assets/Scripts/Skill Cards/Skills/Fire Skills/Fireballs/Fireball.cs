
using UnityEngine;

public class Fireball : CardSpawnable
{
    private float angularSpeed = 90f;
    private float radius;
    private Transform center;
    private float angle;
    public void Init(TowerDataHolder tower, float radius, float startAngle)
    {
        base.Init(tower);
        center = tower.transform;
        this.radius = radius;
        angle = startAngle;
        CallModifier.Call<IFireballModifier>(tower.SkillContext, (mod, instance) =>
        {
            mod.Modify(RuntimeData as FireballEffectData);
        });
    }

    private void Update()
    {
        if (center == null) return;
        angle += angularSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        transform.position = center.position + offset * radius;
        transform.right = offset;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        var e = other.GetComponent<IDamageable>();
        var damageData = new HitData(RuntimeData.damage, tower.GetComponent<TowerShooting>(), e, e.Transform.GetComponent<IUsesStatusEffects>());
        damageData.colors.Add(RuntimeData.color);
        DamageService.ApplyDamage(damageData, e, tower.SkillContext);
        OnDealtDamage?.Invoke(RuntimeData.damage);
        (RuntimeData as FireballEffectData).OnExplode?.Invoke(transform.position);
        Destroy(gameObject);
    }
}
