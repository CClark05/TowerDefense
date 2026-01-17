using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class SkillInstance
{
    protected SkillInstance(SkillData data)
    {
        Data = data;
    }
    public SkillContext skillContext { get; protected set; }
    public SkillData Data { get; }
    
    private int? runtimeStat;
    public int? RuntimeStat
    {
        get => runtimeStat;
        protected set
        {
            if (runtimeStat == value || !value.HasValue) return;
            runtimeStat = value.Value;
            OnRuntimeStatUpdated?.Invoke(value.Value);
        }
    }
    public event Action<int> OnRuntimeStatUpdated;
    private int playCount = 1;
    public int PlayCount
    {
        get => playCount;
        set
        {
            if (playCount == value) return;
            playCount = value;
            OnPlayCountUpdated?.Invoke(value);
        }
    }

    private bool isDisabled;
    public bool IsDisabled
    {
        get => isDisabled;
        set
        {
            if(isDisabled == value) return;
            isDisabled = value;
            OnIsDisabledUpdated?.Invoke(value);
            if (isDisabled)
            {
                Debug.Log("Disabling card: " + Data.name);
                skillContext?.DisableCard(this);
            }
            else skillContext?.EnableCard(this);
        }
    }

    private int damage;
    public int Damage
    {
        get => damage;
        set
        {
            if (damage == value) return;
            damage = value;
            OnDamageUpdated?.Invoke(value);
        }
    }

    public event Action<int> OnDamageUpdated;
    private bool laminated;
    public bool Laminated
    {
        get => laminated;
        set
        {
            if (laminated == value) return;
            laminated = value;
            OnIsLaminatedUpdated?.Invoke(value);
        }
    }
    public event Action <bool> OnIsDisabledUpdated;
    public event Action<bool> OnIsLaminatedUpdated;
    public event Action OnDispose;
    public void SetContext(SkillContext context) => skillContext = context;
    public event Action OnPlayCard;
    public event Action<int> OnPlayCountUpdated;
    
    protected void PlayCard() => OnPlayCard?.Invoke();
    public virtual void Dispose()
    {
        Debug.Log("dispose called on " + Data.name);
        OnDispose?.Invoke();
        OnPlayCard = null;
        OnPlayCountUpdated = null;
        OnIsLaminatedUpdated = null;
        OnIsDisabledUpdated = null;
        skillContext = null;
        OnDispose = null;
        OnRuntimeStatUpdated = null;
        OnDamageUpdated = null;
    }
    protected List<TowerDataHolder> GetTowersInRange()
    {
        return TowerDataHolder.ActiveTowerList.Where(
            t => t.SkillContext != skillContext && 
                 Vector2.Distance(t.transform.position, skillContext.Tower.transform.position) <= skillContext.Tower.RuntimeData.Range).ToList();
    }
}
public abstract class SkillInstance<TData> : SkillInstance where TData : SkillData
{
    protected new TData Data;
    protected SkillInstance(TData data) : base(data)
    {
        Data = data;
        if (data.statusEffects.Any(e => e.data is LaminatedEffectData))
            Laminated = true;
    }
}

public class SupportSkillInstance<TData> : SkillInstance<TData>, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>, IOnNewTowerAdded, IPlayCountPolicy<IOnNewTowerAdded>, IOnRangeUpdated, IPlayCountPolicy<IOnRangeUpdated> where TData : SkillData
{
    protected readonly HashSet<TowerDataHolder> affectedTowers = new();
    public event Action<TowerDataHolder> OnApply;
    public event Action<TowerDataHolder> OnRemove;
    protected SupportSkillInstance(TData data) : base(data)
    {
    }

    private List<TowerDataHolder> GetTowers()
    {
        return TowerDataHolder.ActiveTowerList.Where(
            t => t.SkillContext != skillContext && Vector2.Distance(t.transform.position, skillContext.Tower.transform.position) <= skillContext.Tower.RuntimeData.Range).ToList();
    }
    
    public void Apply(TowerWaveData towerWaveData)
    {
        var towers = GetTowers();
        foreach (var tower in towers)
        {
            OnApply?.Invoke(tower);
            affectedTowers.Add(tower);
        }
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        foreach (var tower in affectedTowers)
            OnRemove?.Invoke(tower);
        affectedTowers.Clear();
    }
    
    public void OnNewTowerAdded(TowerDataHolder towerDataHolder)
    {
        if (Vector2.Distance(towerDataHolder.transform.position, skillContext.Tower.transform.position) <= skillContext.Tower.RuntimeData.Range)
        {
            OnApply?.Invoke(towerDataHolder);
            affectedTowers.Add(towerDataHolder);
        }
    }
    
    public void OnRangeUpdated()
    {
        var towers = TowerDataHolder.ActiveTowerList.Where(
            t => t.SkillContext != skillContext && Vector2.Distance(t.transform.position, skillContext.Tower.transform.position) <= skillContext.Tower.RuntimeData.Range);
        foreach (var tower in towers.Except(affectedTowers).ToList())
        {
            OnApply?.Invoke(tower);
            affectedTowers.Add(tower);
        }

        foreach (var tower in affectedTowers.Except(towers).ToList())
        {
            OnRemove?.Invoke(tower);
            affectedTowers.Remove(tower);
        }
    }

    public virtual int SetPlayCount() => 1;
    
    
}