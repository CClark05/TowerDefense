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
    public event Action <bool> OnIsDisabledUpdated;
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
        OnIsDisabledUpdated = null;
        skillContext = null;
        OnDispose = null;
        OnRuntimeStatUpdated = null;
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