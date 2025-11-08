using System;

public static class CallModifier
{
    public static void Call<TModifier>(SkillContext context, Action<TModifier, SkillInstance> call) where TModifier : class
    {
        foreach (var mod in context.GetSkillInstancesWith<TModifier>())
        {
            if(mod.instance.IsDisabled) continue;
            int playCount = mod.instance is IPlayCountPolicy<TModifier> policy ? policy.SetPlayCount() : mod.instance.PlayCount;
            for(int i = 0; i < playCount; i++)
            {
                call?.Invoke(mod.modifier, mod.instance);
            }
        }
    }
    
    public static void Call<TModifier>(SkillContext context, Action<TModifier, SkillInstance> call, Action<TModifier> OnComplete) where TModifier : class
    {
        foreach (var mod in context.GetSkillInstancesWith<TModifier>())
        {
            if(mod.instance.IsDisabled) continue;
            int playCount = mod.instance is IPlayCountPolicy<TModifier> policy ? policy.SetPlayCount() : mod.instance.PlayCount;
            for(int i = 0; i < playCount; i++)
            {
                call?.Invoke(mod.modifier, mod.instance);
            }
            OnComplete?.Invoke(mod.modifier);
        }
    }
    
    public static bool TryCall<TModifier>(SkillInstance instance, Action<TModifier, SkillInstance> call, bool ignoreDisabled = false) where TModifier : class
    {
        if (instance is TModifier modifier && (ignoreDisabled || !instance.IsDisabled))
        {
            int playCount = instance is IPlayCountPolicy<TModifier> policy ? policy.SetPlayCount() : instance.PlayCount;
            for (int i = 0; i < playCount; i++)
            {
                call?.Invoke(modifier, instance);
            }
            return true;
        }
        return false;
    }
}
