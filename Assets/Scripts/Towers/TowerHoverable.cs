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
        if (EnemyManager.Instance.WaveState is EnemyManager.WaveStates.Spawning || EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning)
        {
            Debug.Log("test");
            return;
        }

        if (CardSelectUI.Instance.IsActive)
        {
            Debug.Log("test2");
            return;
        }
        OnClickTowerStatic?.Invoke(GetComponent<TowerDataHolder>());
        OnClickTower?.Invoke();
        Debug.Log("test3");
    }

    
}