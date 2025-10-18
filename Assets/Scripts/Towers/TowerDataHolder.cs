using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerDataHolder : MonoBehaviour, IBuffOverride, ITowerStatsProvider
{
    [SerializeField] private TowerData baseData;
    public TowerData Data => baseData;
    public string ID { get; private set; }
    public TowerRuntimeData RuntimeData { get; private set; }
    public TowerWaveData WaveData;
    [SerializeField] private ProjectileData projectileData;
    public ProjectileData ProjectileData => projectileData;
    public int EnemiesKilled { get; private set; }
    public int TotalDamage { get; private set; }
    public int GoldValue { get; private set; }
    private TowerShooting towerShooting;
    [SerializeField] private TowerSelectUI towerSelectUI;
    public event Action OnUpdateStats;
    public event Action OnUpdateCards;
    public SkillContext SkillContext { get; private set; }
    public List<SkillInstance> SkillInstanceList => SkillContext.ActiveSkills;
    public static List<TowerDataHolder> ActiveTowerList { get; private set; } = new();
    private EnemyManager enemyManager;
    private void Awake()
    {
        towerShooting = GetComponent<TowerShooting>();
        GoldValue = baseData.cost;
        SkillContext = new SkillContext(this);
        RuntimeData = baseData.ToRuntime();
        if (!ActiveTowerList.Contains(this)) 
            ActiveTowerList.Add(this);
        ID = Guid.NewGuid().ToString("N").Substring(0, 6);
    }
    private void Start()
    {
        towerShooting.OnDealDamage += OnDealDamage;
        towerShooting.OnKillEnemy += OnKillEnemy;
        towerSelectUI.OnSellCard += (data) => TryRemoveCard(data);
        enemyManager = EnemyManager.Instance;
        enemyManager.OnWaveStarted += OnWaveStart;
        enemyManager.OnWaveComplete += OnWaveComplete;
        GetComponent<TowerCards>().OnAddedCard += (data) => AddCard(data); 
        GetComponent<TowerCards>().OnRemovedCard += (data) => TryRemoveCard(data);
        SkillContext.OnTowerUpdated += UpdateTowerData;
    }
    private void OnWaveStart()
    {
        TowerService.ModifyWaveStart(WaveData, SkillContext);
        PlayerService.ModifyWaveStart(SkillContext);
        TowerService.MarkWaveStartDone(this);
    }
    private void OnWaveComplete()
    {
        TotalWaveDamage = 0;
        RealWaveDPS = 0;
        MaxWaveDPS = 0;
        TowerWaveData waveData = new TowerWaveData
        {
            borrowRequests = WaveData.borrowRequests
        };
        TowerService.ModifyWaveEnd(waveData, SkillContext);
        PlayerService.ModifyWaveEnd(SkillContext);
        UpdateTowerData(waveData);
        TowerService.MarkWaveEnd(this);
    }
    public void UpdateTowerData(TowerWaveData towerWaveData)
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
        if(towerWaveData.stunnedDuration > 0)
        {
            RuntimeData.stunned = true;
            FunctionTimer.Create(() =>
            {
                RuntimeData.stunned = false;
            }, towerWaveData.stunnedDuration);
        }
        int activeSlots = SkillInstanceList.FindAll(s => !s.IsDisabled).Count;
        if (RuntimeData.CardSlots < activeSlots)
        {
            int cardsToDisable = activeSlots - RuntimeData.CardSlots;
            for(int i = SkillInstanceList.Count - 1; i >= 0 && cardsToDisable > 0; i--)
            {
                var skill = SkillInstanceList[i];
                if (!skill.IsDisabled)
                {
                    skill.IsDisabled = true;
                    cardsToDisable--;
                }
            }
        }
    }

    private float TotalWaveTime;
    public float RealWaveDPS { get; private set; }
    public float MaxWaveDPS { get; private set; } 
    public float AverageWaveDPS { get; private set; }
    public float AverageOverallDps { get; private set; }
    public float UptimePercentage { get; private set; }
    public float TotalWaveDamage { get; private set; }
    private float dpsTimer;
    private float damageThisSecond;
    private void Update()
    {
        if (EnemyManager.Instance.WaveState is EnemyManager.WaveStates.Idle or EnemyManager.WaveStates.Complete) return;
        TotalWaveTime += Time.deltaTime;   
        dpsTimer += Time.deltaTime;
        if (dpsTimer >= 1)
        {
            RealWaveDPS = damageThisSecond / dpsTimer;
            AverageWaveDPS = TotalWaveDamage / EnemyManager.Instance.WaveTimer;
            AverageOverallDps = TotalDamage / TotalWaveTime;
            UptimePercentage = towerShooting.TotalUptime / TotalWaveTime;
            if (RealWaveDPS > MaxWaveDPS)
                MaxWaveDPS = RealWaveDPS;
            damageThisSecond = 0;
            dpsTimer--;
            OnUpdateStats?.Invoke();
        }
    }
    public bool TryRemoveCard(SkillData data)
    {
        if (SkillContext.TryRemoveSkill(data))
        {
            //SkillInstanceList.Remove(data);
            GoldValue -= Mathf.FloorToInt(data.price * 0.5f);
            OnUpdateCards?.Invoke();
            Debug.Log($"Removed card");
            return true;
        }

        return false;
    }
    public void AddCard(SkillData data, int playCount = 1)
    {
        Debug.Log($"[{name}] AddCard {data.name} (PlayCount={playCount})");
        SkillContext.AddSkill(data, playCount);
        //SkillInstanceList.Add(data);
        GoldValue += Mathf.FloorToInt(data.price * 0.5f);
        OnUpdateCards?.Invoke();
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
        TotalWaveDamage += damage;
        OnUpdateStats?.Invoke();
    }
    public void AddBuff(IBuff buff, int stacks) => SkillContext.AddBuff(buff, stacks);

    public void RemoveBuff(IBuff buff, int stacks) => SkillContext.TryRemoveBuff(buff, stacks);

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