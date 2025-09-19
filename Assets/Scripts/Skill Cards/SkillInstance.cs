using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


public abstract class SkillInstance
{
    protected SkillContext skillContext;
    public SkillData Data { get; }
    private bool playTwice;
    public bool PlayTwice
    {
        get => playTwice;
        set
        {
            if (playTwice == value) return;
            playTwice = value;
            OnPlayTwiceUpdated?.Invoke(value);
        }
    }
    protected SkillInstance(SkillData data)
    {
        Data = data;
    }
    public void SetContext(SkillContext context) => skillContext = context;
    public event Action OnPlayCard;
    public event Action<bool> OnPlayTwiceUpdated;
    protected void PlayCard() => OnPlayCard?.Invoke();

    public virtual void Dispose()
    {
        OnPlayCard = null;
        OnPlayTwiceUpdated = null;
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