using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData")]
public class TowerData : GridObjectData
{
    public float timeBetweenShots;
    public float range;
    public int damage;
    public int cardSlots;
    public int cost;
    public TowerRuntimeData ToRuntime() => new TowerRuntimeData {
        timeBetweenShots = timeBetweenShots,
        Range = range,
        CardSlots = cardSlots,
    };
}

public class TowerRuntimeData
{
    public float timeBetweenShots;
    private float range;
    private int cardSlots;
    public bool stunned;
    public float Range
    {
        get => range;
        set
        {
            if (Mathf.Approximately(value, range)) return;
            range = value;
            OnRangeUpdated?.Invoke(value);
        }
    }
    public int CardSlots
    {
        get => cardSlots;
        set
        {
            if (value == cardSlots) return;
            cardSlots = value;
            OnCardSlotsUpdated?.Invoke(value);
        }
    }
    public event Action<float> OnRangeUpdated;
    public event Action<int> OnCardSlotsUpdated;
}