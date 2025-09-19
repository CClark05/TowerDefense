using System;
using UnityEngine;

public class TowerHoverable : MonoBehaviour, IHoverable
{
    public static Action<TowerDataHolder> OnHoverTowerStatic;
    public Action OnHoverTower;
    public static Action OnLeaveTowerHoverStatic;
    public Action OnLeaveHoverTower;
    public static Action<TowerDataHolder> OnClickTowerStatic;
    public Action OnClickTower;
    public bool IgnoreRange => true;
    public void OnHover()
    {
        OnHoverTowerStatic?.Invoke(GetComponent<TowerDataHolder>());
        OnHoverTower?.Invoke();
    }

    public void OnLeaveHover()
    {
        OnLeaveTowerHoverStatic?.Invoke();
        OnLeaveHoverTower?.Invoke();
    }

    public void OnClick()
    {
        OnClickTowerStatic?.Invoke(GetComponent<TowerDataHolder>());
        OnClickTower?.Invoke();
    }

    
}