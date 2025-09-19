
public static class TowerService
{
    public static void ModifyWaveStart(TowerWaveData data, SkillContext context)
    {
        foreach (var mod in context.GetSkillInstancesWith<ITowerWaveStartModifier>())
        {
            mod.modifier.Modify(data);
            if (mod.instance.PlayTwice)
                mod.modifier.Modify(data);
        }
    }

    public static void ModifyWaveEnd(TowerWaveData data, SkillContext context)
    {
        foreach (var mod in context.GetSkillInstancesWith<ITowerWaveEndModifier>())
        {
            mod.modifier.Modify(data);
            if (mod.instance.PlayTwice)
                mod.modifier.Modify(data);
        }
    }

    public static bool TryModifyOnCardReceived(TowerWaveData data, SkillInstance skillInstance)
    {
        if (skillInstance is ITowerCardReceivedModifier modifier)
        {
            modifier.Apply(data);
            if(skillInstance.PlayTwice)
                modifier.Apply(data);
            return true;
        }
        return false;
    }
    
    public static bool TryModifyOnCardRemoved(TowerWaveData data, SkillInstance skillInstance)
    {
        if (skillInstance is ITowerCardReceivedModifier modifier)
        {
            modifier.Remove(data);
            if(skillInstance.PlayTwice)
                modifier.Remove(data);
            return true;
        }
        return false;
    }

    public static void ApplyBuff(IBuff buff, TowerWaveData data, int stacks)
    {
        for(int i = 0; i<stacks; i++)
            buff.Apply(data);
    }
    public static void RemoveBuff(IBuff buff, TowerWaveData data, int stacks)
    {
        for (int i = 0; i < stacks; i++)
            buff.Remove(data);
    }
}