using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShields : MonoBehaviour, IUsesShields
{
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GridLayoutGroup shieldLayout;
    private EnemyData data;
    private List<GameObject> activeShields = new();
    private void Start()
    {
        data = GetComponent<EnemyDataHolder>().Data;
        AddShields(data.shields);
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

    public void AddShields(int amount)
    {
        for (int i = 0; i < data.shields; i++)
        {
            activeShields.Add(Instantiate(shieldPrefab, shieldLayout.transform));
        }
    }
}
