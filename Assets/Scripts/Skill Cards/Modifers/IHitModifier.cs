public interface IHitModifier 
{
    public void Modify(HitData hitData, IDamageable target);

    public enum Priority
    {
        PlusDamage,
        MultDamage,
    }
    public Priority priority { get; }
}