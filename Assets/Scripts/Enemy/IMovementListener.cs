using System;

public interface IMovementListener
{
    public event Action OnReachedEnd;
    public float Progress { get;  }
}