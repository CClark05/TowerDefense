using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : Singleton<PlayerInventory>
{
    [SerializeField] private int startingCoins;
    private int coins;
    public int Coins
    {
        get => coins;
        private set
        {
            if (Equals(value, coins)) return;
            coins = value;
            OnCoinsUpdated?.Invoke(value);
            foreach (var tower in TowerDataHolder.ActiveTowerList)
            {
                CallModifier.Call<IOnCoinsUpdated>(tower.SkillContext, (mod,instance) => mod.OnCoinsUpdated());
                /**
                var context = tower.SkillContext;
                foreach (var mod in context.GetSkillInstancesWith<IOnCoinsUpdated>())
                {
                    for (int i = 0; i < (mod.modifier.alwaysPlayOnce ? 1 : mod.instance.PlayCount); i++)
                    {
                        mod.modifier.OnCoinsUpdated();
                    }
                }
                */
            }
        }
    }
    public int TotalCoinsEarned { get; private set; }
    public event Action<int> OnCoinsUpdated;
    public event Action<int> OnCoinsAdded;
    public event Action<int> OnCoinsRemoved;
    private void Start()
    {
        AddCoins(startingCoins);
        BuildingManager.Instance.OnPlacedBuild += data => SubtractCoins(data.cost);
        TowerSellable.OnSellTower += AddCoins;
        SkillCardUI.OnSellCardStatic += AddCoins;
        TowerSelectUI.OnSellCardStatic += SellCardStatic;
        CardSelectUI.Instance.OnReroll += SubtractCoins;
        EnemyManager.Instance.OnEnemyKilled += AddCoins;
        ShopCardUI.OnBuyCardStatic += data => SubtractCoins(data.price);
        SpinToWinUI.Instance.OnPlacedWager += SubtractCoins;
        SpinToWinUI.Instance.OnWagerComplete += AddCoins;
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