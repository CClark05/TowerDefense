using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HashSet wrapper that raises events when it changes.
/// </summary>
public class ObservableHashSet<T> : ICollection<T>, IReadOnlyCollection<T>
{
    private readonly HashSet<T> _set;

    public event Action<T> OnItemAdded;
    public event Action<T> OnItemRemoved;
    public event Action OnCleared;

    public int Count => _set.Count;
    public bool IsReadOnly => false;

    public ObservableHashSet()
    {
        _set = new HashSet<T>();
    }

    public ObservableHashSet(IEqualityComparer<T> comparer)
    {
        _set = new HashSet<T>(comparer);
    }

    /// <summary>
    /// Adds an item; returns true only if it was not already present.
    /// </summary>
    public bool Add(T item)
    {
        if (!_set.Add(item))
            return false;

        OnItemAdded?.Invoke(item);
        return true;
    }

    // ICollection<T>.Add explicit implementation
    void ICollection<T>.Add(T item) => Add(item);

    public bool Remove(T item)
    {
        if (!_set.Remove(item))
            return false;

        OnItemRemoved?.Invoke(item);
        return true;
    }

    public void Clear()
    {
        if (_set.Count == 0)
            return;

        _set.Clear();
        OnCleared?.Invoke();
    }

    public bool Contains(T item) => _set.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _set.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public void UnionWith(IEnumerable<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        foreach (var item in other)
        {
            Add(item);
        }
    }
}