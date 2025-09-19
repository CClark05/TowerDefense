using System;
using UnityEngine;

public class TowerSellable : MonoBehaviour, ISellable
{
    public static event Action<int> OnSellTower;
    public void Sell()
    {
        OnSellTower?.Invoke(GetComponent<TowerDataHolder>().GoldValue);
        GridManager.Instance.Grid.GetXY(transform.position, out int x, out int y);
        GridManager.Instance.SetEmpty(x, y);
        Destroy(gameObject);
    }
}