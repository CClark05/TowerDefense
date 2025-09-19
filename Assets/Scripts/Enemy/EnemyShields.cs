using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShields : MonoBehaviour, IUsesShields
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GridLayoutGroup shieldLayout;
    private EnemyData data;
    public int ShieldCount { get; private set; }
    private List<GameObject> activeShields = new();
    private void Start()
    {
        data = GetComponent<EnemyDataHolder>().Data;
        for (int i = 0; i < data.shields; i++)
        {
            activeShields.Add(Instantiate(shieldPrefab, shieldLayout.transform));
        }
        ShieldCount = data.shields;
    }

   
    public bool TryRemoveShield(int amount)
    {
        bool removed = false;
        for (int i = 0; i < amount && activeShields.Count > 0; i++)
        {
            int lastIndex = activeShields.Count - 1;
            var shield = activeShields[lastIndex];
            activeShields.RemoveAt(lastIndex);
            Destroy(shield);
            removed = true;
        }

        return removed;
    }

    public void AddShield(int amount)
    {
        activeShields.Add(Instantiate(shieldPrefab, shieldLayout.transform));
    }
}
