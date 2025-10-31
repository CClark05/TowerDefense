using System;

public interface IUsesShields
{
    public bool TryRemoveShield(int amount);
    public void AddShields(int amount);
    public event Action<int> OnShieldCountChanged;
}