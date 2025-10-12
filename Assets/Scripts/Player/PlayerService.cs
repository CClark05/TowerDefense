public static class PlayerService
{
    public static void ModifyWaveStart(SkillContext skillContext)
    {
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
        foreach (var mod in skillContext.GetSkillInstancesWith<IPlayerWaveStartModifier>())
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.WaveEnd();
            }
        }
    }
}
