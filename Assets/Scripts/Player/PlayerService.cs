public static class PlayerService
{
    /**
    public static void ModifyWaveStart(SkillContext skillContext)
    {
        CallModifier.Call<IPlayerWaveStartModifier>(skillContext, (mod, _) =>
        {
            mod.WaveStart();
        });
        /**
        foreach (var mod in skillContext.GetSkillInstancesWith<IPlayerWaveStartModifier>())
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.WaveStart();
            }
        }
    
    }

    public static void ModifyWaveEnd(SkillContext skillContext)
    {
        CallModifier.Call<IPlayerWaveStartModifier>(skillContext, (mod, _) =>
        {
            mod.WaveEnd();
        });
        /**
        foreach (var mod in skillContext.GetSkillInstancesWith<IPlayerWaveStartModifier>())
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.WaveEnd();
            }
        }
    }
    */
}
