public interface IUsesShields
{
    public int ShieldCount { get; }
    public bool TryRemoveShield(int amount);
    public void AddShield(int amount);
}