using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Boomerang Data", menuName = "SkillData/Generic/Boomerang")]
public class BoomerangSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Instead of being destroyed projectiles will return back, hitting enemies on the way.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BoomerangSkillInstance(this);
    }
}

public class BoomerangSkillInstance : SkillInstance<BoomerangSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public BoomerangSkillInstance(BoomerangSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        shotData.OnShotDestroyed += p =>
        {
            CoroutineRunner.Instance.StartCoroutine(ReturnProjectile(p));
        };
        yield return null;
    }
    
    private IEnumerator ReturnProjectile(Projectile projectile)
    {
        var transform = projectile.transform;

        while (transform != null && Vector2.Distance(transform.position, projectile.Origin) > 0.1f)
        {
            Vector2 dir = (projectile.Origin - (Vector2)transform.position).normalized;
            transform.position += (Vector3)dir * (projectile.EffectiveSpeed * Time.deltaTime);
            yield return null;
        }

        if (transform != null)
            UnityEngine.Object.Destroy(transform.gameObject);
    }

    
    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}