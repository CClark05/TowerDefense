using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInventory : Singleton<PlayerInventory>
{
    [SerializeField] private int startingCoins;
    [SerializeField] private Transform coinUI;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private AnimationClip coinFlipAnimation;
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
        //TowerSelectUI.OnSellCardStatic += SellCardStatic;
        CardSelectUI.Instance.OnReroll += SubtractCoins;
        EnemyManager.Instance.OnEnemyKilled += AddCoins;
        ShopCardUI.OnBuyCardStatic += data => SubtractCoins(data.price);
        SpinToWinUI.Instance.OnPlacedWager += SubtractCoins;
        SpinToWinUI.Instance.OnWagerComplete += AddCoins;
        ShopSellPanel.Instance.OnSellCard += SellCard;
    }
    private void SellCard(SkillInstance instance) => AddCoins(Mathf.RoundToInt(instance.Data.price * 0.5f));
    
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
    public void AddCoins(int amount, Vector2 position)
    {
        OnCoinsAdded?.Invoke(amount);
        Coins += amount;
        TotalCoinsEarned += amount;
        CoinAnimation(amount, position);
    }

    public void CoinAnimation(int amount, Vector2 position)
    {
        StartCoroutine(CoinAnimation());
        IEnumerator CoinAnimation()
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 rand = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0, 1f));
                var coin = Instantiate(coinPrefab, position + rand, Quaternion.identity);
                coin.GetComponent<IAnimationPlayer>().Play(coinFlipAnimation, coin.transform);
                var sequence = DOTween.Sequence();
                float moveAmount = 0.4f;
                float moveDuration = 0.25f;
                sequence.Append(coin.transform.DOMoveY(moveAmount, moveDuration).SetEase(Ease.OutCubic).SetRelative(true));
                sequence.AppendInterval(0.025f);
                sequence.Append(coin.transform.DOMoveY(-moveAmount, moveDuration).SetEase(Ease.InCubic).SetRelative(true));
                var start = coin.transform.position;
                float flyDur = 0.45f;
                float arcHeight = Random.Range(0.8f, 1.3f);
                float arcSide = Random.Range(-0.6f, 0.6f);
                Vector3 mid = (coin.transform.position + coinUI.position) * 0.5f + Vector3.up * arcHeight + Vector3.right * arcSide;
                sequence.Append(coin.transform.DOPath(new[] { start, mid, coinUI.transform.position }, flyDur, PathType.CatmullRom)
                    .SetEase(Ease.InOutCubic));
                sequence.Join(coin.transform.DORotate(new Vector3(0, 0, Random.Range(-540f, 540f)), flyDur, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutCubic));
                sequence.Join(coin.transform.DOScale(1.1f, flyDur * 0.2f).SetEase(Ease.OutBack));
                sequence.Join(coin.GetComponent<SpriteRenderer>().DOFade(0, 0.2f).SetDelay(flyDur - 0.2f));
                sequence.OnComplete(() => Destroy(coin));
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
    public void SubtractCoins(int amount)
    {
        OnCoinsRemoved?.Invoke(amount);
        Coins -= amount;
    }
    
}