using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


public abstract class SkillInstance
{
    protected SkillContext skillContext;
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
        }
    }
    protected SkillInstance(SkillData data)
    {
        Data = data;
    }
    public void SetContext(SkillContext context) => skillContext = context;
    public event Action OnPlayCard;
    public event Action<int> OnPlayCountUpdated;
    protected void PlayCard() => OnPlayCard?.Invoke();

    public virtual void Dispose()
    {
        OnPlayCard = null;
        OnPlayCountUpdated = null;
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