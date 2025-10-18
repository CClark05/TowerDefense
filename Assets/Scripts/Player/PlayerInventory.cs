using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : Singleton<PlayerInventory>
{
    private int coins;
    public int Coins
    {
        get => coins;
        private set
        {
            if (Equals(value, coins)) return;
            coins = value;
            OnCoinsUpdated?.Invoke(value);
        }
    }
    public int TotalCoinsEarned { get; private set; }
    public event Action<int> OnCoinsUpdated;
    public event Action<int> OnCoinsAdded;
    public event Action<int> OnCoinsRemoved;
    private void Start()
    {
        BuildingManager.Instance.OnPlacedBuild += data => SubtractCoins(data.cost);
        TowerSellable.OnSellTower += AddCoins;
        SkillCardUI.OnSellCardStatic += AddCoins;
        TowerSelectUI.OnSellCardStatic += SellCardStatic;
        CardSelectUI.Instance.OnReroll += SubtractCoins;
        EnemyManager.Instance.OnEnemyKilled += AddCoins;
    }
    private void SellCardStatic(SkillData data) => AddCoins(Mathf.FloorToInt(data.price * 0.5f));
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            AddCoins(10);
    }
    public void AddCoins(int amount)
    {
        OnCoinsAdded?.Invoke(amount);
        Coins += amount;
        TotalCoinsEarned += amount;
    }

    private void SubtractCoins(int amount)
    {
        OnCoinsRemoved?.Invoke(amount);
        Coins -= amount;
    }
    
}