using System;
using UnityEngine;

public class Button_Scale_Hover : Button_Scale
{
    public event Action OnHover;
    public event Action OnLeaveHover;
    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        OnHover?.Invoke();
    }

    public override void OnMouseLeave()
    {
        base.OnMouseLeave();
        OnLeaveHover?.Invoke();
    }
}