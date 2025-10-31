using System;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private ShopSettings settings;
    [SerializeField] private GameObject shopVisual;
    public event Action OnCreateShop;
    private void Start()
    {
        shopVisual.SetActive(false);
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            if ((EnemyManager.Instance.CurrentWave - 1) % settings.shopEveryXWaves == 0)
            {
                Debug.Log("SHOP");
                shopVisual.SetActive(true);
                OnCreateShop?.Invoke();
            }
        };
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            shopVisual.SetActive(false);
        };
    }
}
