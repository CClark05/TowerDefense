using System;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyDataHolder>(out var enemy))
        {
            PlayerInventory.Instance.SubtractCoins(enemy.Data.livesCost);
        }
    }
}
