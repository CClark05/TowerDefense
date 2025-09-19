public interface IOnEffectApplied
{
    public void Modify(ModifyEffectData effectData, HitData hitData);
    public EffectData Effect { get; }
}

public class ModifyEffectData
{
    public int additionalStacks;
}