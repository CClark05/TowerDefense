using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData")]
public class TowerData : GridObjectData
{
    public float timeBetweenShots;
    public float range;
    public int damage;
    public int cardSlots;
    public int cost;
    public int Cost
    {
        get => cost;
        set
        {
            if (value == cost) return;
            cost = value;
            OnCostIncreased?.Invoke(value);
        }
    }
    [SerializeField, Min(1f)] private float growthRate = 1.28f;
    public int costIncreasePerPurchase => Mathf.Max(1,
        Mathf.RoundToInt(Cost * (growthRate - 1f)));
    public event Action<int> OnCostIncreased;
    public TowerRuntimeData ToRuntime() => new TowerRuntimeData {
        TimeBetweenShots = timeBetweenShots,
        Range = range,
        BaseDamage = damage,
        CardSlots = cardSlots,
    };
}

public class TowerRuntimeData
{
    private float timeBetweenShots;
    private float range;
    private int baseDamage;
    private int cardSlots;
    public bool stunned;
    public float Range
    {
        get => range;
        set=> SetProperty(ref range, value, OnRangeUpdated);
    }
    public int CardSlots
    {
        get => cardSlots;
        set => SetProperty(ref cardSlots, value, OnCardSlotsUpdated);
    }
    public int BaseDamage 
    {
        get => baseDamage;
        set => SetProperty(ref baseDamage, value, OnBaseDamageUpdated);
    }
    public float TimeBetweenShots
    {
        get => timeBetweenShots;
        set => SetProperty(ref timeBetweenShots, value);
    }
    public event Action<float> OnRangeUpdated;
    public event Action<int> OnCardSlotsUpdated;
    public event Action<int> OnBaseDamageUpdated;
    public event Action OnStatsUpdated;
    private void SetProperty<T>(ref T field, T value, Action<T> onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        onChanged?.Invoke(value);
        OnStatsUpdated?.Invoke();
    }
}