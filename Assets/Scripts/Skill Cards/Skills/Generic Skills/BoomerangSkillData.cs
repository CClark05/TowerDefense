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
        Transform transform = null;
        shotData.OnShotDestroyed += () =>
        {
            transform = shotData.projectile.transform;
        };
        CoroutineRunner.Instance.StartCoroutine(Return());
        yield return null;
        
        IEnumerator Return()
        {
            yield return new WaitUntil(() => transform != null);
            while (transform != null && Vector2.Distance(transform.position, shotData.projectile.Origin) > 0.1f)
            {
                Vector2 direction = (shotData.projectile.Origin - (Vector2)transform.position).normalized;
                transform.position += (Vector3)direction * (shotData.projectile.EffectiveSpeed * Time.deltaTime);
                yield return null;
            }
            UnityEngine.Object.Destroy(transform.gameObject);
        }
    }

    
    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}