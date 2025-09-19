using System.Collections;
using System.Linq;


public static class ProjectileService
{
    public static IEnumerator ModifyProjectile(ProjectileShotData shotData, SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<IProjectileModifier>().Where(m => !m.modifier.DelayShot))
        {
            var r1 = mod.modifier.Modify(shotData);
            if (r1 != null) while (r1.MoveNext()) { }  

            if (mod.instance.PlayTwice)
            {
                var r2 = mod.modifier.Modify(shotData);
                if (r2 != null) while (r2.MoveNext()) { } 
            }
        }
        foreach (var mod in skillContext.GetSkillInstancesWith<IProjectileModifier>().Where(m => m.modifier.DelayShot))
        {
            var routine = mod.modifier.Modify(shotData);
            if (routine != null)
                yield return routine;
            
            if (mod.instance.PlayTwice)
            {
                var second = mod.modifier.Modify(shotData);
                if (second != null)
                    yield return second;
            }
        }
    }
}