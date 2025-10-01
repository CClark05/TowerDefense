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
    private Dictionary<ResourceData, int> materialAmounts = new();
    public event Action<ResourceData, int> OnMaterialAmountUpdated;
    public event Action<ResourceData> OnNewMaterialAdded;
    [SerializeField] private PlayerWaveRewardSettings waveRewardSettings;
    public event Action<WaveRewards, bool> OnWaveRewardsCalculated;
    private void Start()
    {
        Resource.OnDroppedMaterial += (data, amount) =>
        {
            if (!materialAmounts.ContainsKey(data))
                OnNewMaterialAdded?.Invoke(data);
            
            materialAmounts[data] = materialAmounts.GetValueOrDefault(data) + amount;
            OnMaterialAmountUpdated?.Invoke(data, materialAmounts[data]);
        };
        BuildingManager.Instance.OnPlacedBuilding += costDict =>
        {
            foreach (var kvp in costDict)
            {
                if (!materialAmounts.ContainsKey(kvp.Key)) continue;
                materialAmounts[kvp.Key] = Mathf.Max(materialAmounts[kvp.Key] - kvp.Value, 0);
                OnMaterialAmountUpdated?.Invoke(kvp.Key, materialAmounts[kvp.Key]);
            }
        };
        TowerSellable.OnSellTower += AddCoins;
        SkillCardUI.OnSellCardStatic += AddCoins;
        TowerSelectUI.OnSellCardStatic += SellCardStatic;
        CardSelectUI.Instance.OnReroll += SubtractCoins;
        EnemyManager.Instance.OnEnemyKilled += AddCoins;
    }
    

    private void SellCardStatic(SkillData data) => AddCoins(Mathf.FloorToInt(data.price * 0.5f));
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Coins += 10;
    }
    
    public int GetAmount(ResourceData data)
    {
        return materialAmounts.GetValueOrDefault(data, 0);
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