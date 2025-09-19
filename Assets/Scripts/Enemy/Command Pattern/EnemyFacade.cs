using System.Collections.Generic;
using UnityEngine;

public class EnemyFacade : MonoBehaviour, IAgent
{
    private readonly Dictionary<System.Type, object> cache = new();
    public Transform Transform => transform;
    public T Get<T>() where T : class
    {
        var type = typeof(T);
        if (cache.TryGetValue(type, out var obj)) return (T)obj;
        if (this is T self)
        {
            cache[type] = self;
            return self;
        }
        var components = GetComponents<Component>();      
        foreach (var component in components)
        {
            if (component is T t)
            {
                cache[type] = t;
                return t;
            }
        }
        return null; 
    }

    public T Require<T>() where T : class => Get<T>() ?? throw new System.InvalidOperationException(
        $"Missing required component {typeof(T).Name} on {name}");
}

