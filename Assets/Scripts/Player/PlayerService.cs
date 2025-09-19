public static class PlayerService
{
    public static void ModifyWaveStart(SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<IPlayerWaveStartModifier>())
        {
            mod.modifier.WaveStart();
            if (mod.instance.PlayTwice)
                mod.modifier.WaveStart();
        }
    }

    public static void ModifyWaveEnd(SkillContext skillContext)
    {
        foreach (var mod in skillContext.GetSkillInstancesWith<IPlayerWaveStartModifier>())
        {
            mod.modifier.WaveEnd();
            if (mod.instance.PlayTwice)
                mod.modifier.WaveEnd();
        }
    }
}
