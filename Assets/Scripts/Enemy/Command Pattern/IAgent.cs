using UnityEngine;

public interface IAgent
{
    Transform Transform { get; }
    T Get<T>() where T : class;
    T Require<T>() where T : class;
}