using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


public abstract class SkillInstance
{
    protected SkillInstance(SkillData data)
    {
        Data = data;
    }
    public SkillContext skillContext { get; protected set; }
    public SkillData Data { get; }
    private int originalPlayCount = 1;
    private int playCount = 1;
    public int PlayCount
    {
        get => playCount;
        set
        {
            if (value > 0)
                originalPlayCount = value;
            if (playCount == value) return;
            if (value > 0 && playCount == 0)
            {
                playCount = value;
                TowerWaveData waveData = new TowerWaveData();
                if (TowerService.TryModifyOnCardReceived(waveData, this))
                {
                    OnUpdateTower?.Invoke(waveData);
                    Debug.Log("Invoked OnUpdateTower for " + Data.name);
                }
                    
            }
            else if(value <= 0 && playCount > 0)
            {
                TowerWaveData waveData = new TowerWaveData();
                if (TowerService.TryModifyOnCardRemoved(waveData, this))
                {
                    OnUpdateTower?.Invoke(waveData);
                    Debug.Log("Invoked OnUpdateTower for removal of " + Data.name);
                }
                playCount = value;
            }
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
            PlayCount = isDisabled ? 0 : originalPlayCount;
        }
    }
    public event Action <bool> OnIsDisabledUpdated;
    public void SetContext(SkillContext context) => skillContext = context;
    public event Action OnPlayCard;
    public event Action<int> OnPlayCountUpdated;
    public event Action<TowerWaveData> OnUpdateTower;
    
    protected void PlayCard() => OnPlayCard?.Invoke();
    public virtual void Dispose()
    {
        Debug.Log("dispose called on " + Data.name);
        OnPlayCard = null;
        OnPlayCountUpdated = null;
        OnIsDisabledUpdated = null;
        OnUpdateTower = null;
        skillContext = null;
    }
}
public abstract class SkillInstance<TData> : SkillInstance where TData : SkillData
{
    protected new TData Data;
    protected SkillInstance(TData data) : base(data)
    {
        Data = data;
    }
}