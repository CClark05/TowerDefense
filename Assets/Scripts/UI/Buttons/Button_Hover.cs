using System;

public class Button_Hover : Button_Base
{
    public event Action OnHover;
    public event Action OnLeaveHover;
    public override void OnMouseEnter()
    {
        OnHover?.Invoke();
    }

    public override void OnMouseLeave()
    {
        OnLeaveHover?.Invoke();
    }
    
}