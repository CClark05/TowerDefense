using System;
using UnityEngine;


public abstract class SkillInstance
{
    protected SkillInstance(SkillData data)
    {
        Data = data;
    }
    public SkillContext skillContext { get; protected set; }
    public SkillData Data { get; }
    private int playCount = 1;
    public int PlayCount
    {
        get => playCount;
        set
        {
            if (playCount == value) return;
            playCount = value;
            OnPlayCountUpdated?.Invoke(value);
            /**
            TowerWaveData waveData = new TowerWaveData();
                if (TowerService.TryModifyOnCardReceived(waveData, this))
                {
                    OnUpdateTower?.Invoke(waveData);
                    Debug.Log("Invoked OnUpdateTower for " + Data.name);
                }
                */
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
    public event Action <bool> OnIsDisabledUpdated;
    public event Action OnDispose;
    public void SetContext(SkillContext context) => skillContext = context;
    public event Action OnPlayCard;
    public event Action<int> OnPlayCountUpdated;
    public event Action<TowerWaveData> OnUpdateTower;
    
    protected void PlayCard() => OnPlayCard?.Invoke();
    public virtual void Dispose()
    {
        Debug.Log("dispose called on " + Data.name);
        OnDispose?.Invoke();
        OnPlayCard = null;
        OnPlayCountUpdated = null;
        OnIsDisabledUpdated = null;
        OnUpdateTower = null;
        skillContext = null;
        OnDispose = null;
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