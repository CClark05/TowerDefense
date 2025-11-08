using System.Collections;
using System.Linq;


public static class ProjectileService
{
    public static IEnumerator ModifyProjectile(ProjectileShotData shotData, SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<IProjectileModifier>().Where(m => !m.modifier.DelayShot))
        {
            if(mod.instance.IsDisabled) continue;
            int playCount = mod.instance is IPlayCountPolicy<IProjectileModifier> policy ? policy.SetPlayCount() : mod.instance.PlayCount;
            for (int i = 0; i < playCount; i++)
            {
                var r = mod.modifier.Modify(shotData);
                if (r != null)
                    while (r.MoveNext()) { }
            }
        }

        foreach (var mod in skillContext.GetSkillInstancesWith<IProjectileModifier>().Where(m => m.modifier.DelayShot))
        {
            if(mod.instance.IsDisabled) continue;
            int playCount = mod.instance is IPlayCountPolicy<IProjectileModifier> policy ? policy.SetPlayCount() : mod.instance.PlayCount;
            for (int i = 0; i < playCount; i++)
            {
                var r = mod.modifier.Modify(shotData);
                if (r != null) yield return r;
            }
        }
    }
}