using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerDataHolder : MonoBehaviour
{
    [SerializeField] private TowerData baseData;
    public TowerData Data => baseData;
    public TowerRuntimeData RuntimeData { get; private set; }
    [SerializeField] private ProjectileData projectileData;
    public ProjectileData ProjectileData => projectileData;
    public int Level { get; private set; } = 1;
    public int EnemiesKilled { get; private set; }
    public int TotalDamage { get; private set; }
    public int GoldValue { get; private set; }
    private TowerShooting towerShooting;
    [SerializeField] private TowerSelectUI towerSelectUI;
    public event Action OnUpdateStats;
    public event Action OnUpdateCards;
    public SkillContext SkillContext { get; private set; }
    public List<SkillData> SkillDataList { get; private set; } = new();
    public static List<TowerDataHolder> ActiveTowerList { get; private set; } = new();
    private EnemyManager enemyManager;
    private void Awake()
    {
        towerShooting = GetComponent<TowerShooting>();
        GoldValue = baseData.buildableData.CalculateGoldValue();
        SkillContext = new SkillContext(this);
        RuntimeData = baseData.ToRuntime();
        if (!ActiveTowerList.Contains(this)) 
            ActiveTowerList.Add(this);
    }
    private void Start()
    {
        
        towerShooting.OnDealDamage += OnDealDamage;
        towerShooting.OnKillEnemy += OnKillEnemy;
        towerSelectUI.OnSellCard += TryRemoveCard;
        enemyManager = EnemyManager.Instance;
        enemyManager.OnWaveStarted += OnWaveStart;
        enemyManager.OnWaveComplete += OnWaveComplete;
        GetComponent<TowerCards>().OnAddedCard += AddCard; 
        GetComponent<TowerCards>().OnRemovedCard += TryRemoveCard;
        SkillContext.OnTowerUpdated += UpdateTowerData;
    }
    private void OnWaveStart()
    {
        TowerWaveData towerWaveData = new TowerWaveData();
        TowerService.ModifyWaveStart(towerWaveData, SkillContext);
        PlayerService.ModifyWaveStart(SkillContext);
        UpdateTowerData(towerWaveData);
    }
    private void OnWaveComplete(int obj)
    {
        TowerWaveData towerWaveData = new TowerWaveData();
        TowerService.ModifyWaveEnd(towerWaveData, SkillContext);
        PlayerService.ModifyWaveEnd(SkillContext);
        UpdateTowerData(towerWaveData);
    }
    public float RealDPS { get; private set; }
    public float MaxDPS { get; private set; } = 0;
    private float dpsTimer;
    private float damageThisSecond;
    private void Update()
    {
        if (EnemyManager.Instance.WaveState == EnemyManager.WaveStates.Idle) return;
        dpsTimer += Time.deltaTime;
        if (dpsTimer >= 1)
        {
            RealDPS = damageThisSecond / dpsTimer;
            if (RealDPS > MaxDPS)
                MaxDPS = RealDPS;
            damageThisSecond = 0;
            dpsTimer = 0;
            OnUpdateStats?.Invoke();
        }
    }

    public void TryRemoveCard(SkillData data)
    {
        if (SkillContext.TryRemoveSkill(data))
        {
            SkillDataList.Remove(data);
            GoldValue -= Mathf.FloorToInt(data.price * 0.5f);
            OnUpdateCards?.Invoke();
        }
    }
    public void AddCard(SkillData data)
    {
        SkillContext.AddSkill(data);
        SkillDataList.Add(data);
        GoldValue += Mathf.FloorToInt(data.price * 0.5f);
        OnUpdateCards?.Invoke();
    }
    private void UpdateTowerData(TowerWaveData towerWaveData)
    {
        foreach (var card in towerWaveData.addedCards)
        {
            AddCard(card);
        }
        foreach (var card in towerWaveData.removedCards)
        {
            TryRemoveCard(card);
        }
        RuntimeData.Range += towerWaveData.increasedRange;
        RuntimeData.CardSlots += towerWaveData.increasedSlots;
        RuntimeData.timeBetweenShots /= towerWaveData.increasedSpeed;
    }
    private void OnKillEnemy()
    {
        EnemiesKilled++;
        OnUpdateStats?.Invoke();
    }
    private void OnDealDamage(int damage)
    {
        damageThisSecond += damage;
        TotalDamage += damage;
        OnUpdateStats?.Invoke();
    }

    private void OnDisable()
    {
        enemyManager.OnWaveStarted -= OnWaveStart;
        enemyManager.OnWaveComplete -= OnWaveComplete;
    }

    private void OnDestroy()
    {
        ActiveTowerList.Remove(this);
    }
}