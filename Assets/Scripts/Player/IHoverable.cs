using UnityEngine;

public interface IHoverable
{
    public void OnHover();
    public void OnLeaveHover();
    public void OnClick();
    public bool IgnoreRange { get; }
}

