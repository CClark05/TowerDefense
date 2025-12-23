public interface IOnEffectApplied
{
    public void Modify(ModifyEffectData effectData, HitData hitData);
    public EffectData Effect { get; }
    public bool PlayOnce { get; }
}

public class ModifyEffectData
{
    public int additionalStacks;
    public int? duration;
}