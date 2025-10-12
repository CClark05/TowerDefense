using System.Collections;
using System.Linq;


public static class ProjectileService
{
    public static IEnumerator ModifyProjectile(ProjectileShotData shotData, SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<IProjectileModifier>().Where(m => !m.modifier.DelayShot))
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                var r = mod.modifier.Modify(shotData);
                if (r != null) while (r.MoveNext()) { } 
            }
        }
        foreach (var mod in skillContext.GetSkillInstancesWith<IProjectileModifier>().Where(m => m.modifier.DelayShot))
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                var r = mod.modifier.Modify(shotData);
                if (r != null) yield return r; 
            }
        }
    }
}