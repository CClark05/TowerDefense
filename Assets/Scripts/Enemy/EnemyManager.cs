using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<GameObject> currentEnemies = new();
    public List<GameObject> CurrentEnemies => currentEnemies;
    public List<EnemyData> DeadEnemies { get; private set; } = new();
    private LevelData levelData;
    public int CurrentWave { get; private set; } = 1;
    public float WaveTimer { get; private set; }
    public enum WaveStates
    {
        Idle,
        Spawning,
        DoneSpawning,
        Complete
    }

    public WaveStates WaveState { get; private set; } = WaveStates.Idle;

    public event Action OnIdle;
    public event Action OnWaveStarted;
    public event Action OnWaveComplete;
    public event Action<GameOverData> OnNoWavesLeft;
    public event Action OnEnemiesUpdated;
    public event Action<int> OnEnemyReachedEnd;
    public event Action<int> OnEnemyKilled;
    //private WaveData waveData => levelData.waves[CurrentWave - 1];
    [FormerlySerializedAs("waveLibrary")] [SerializeField] private WaveSettings waveSettings;
    private WaveGenerator waveGenerator;
    private void Start()
    {
        levelData = LevelDataHolder.Instance.Data;
        OnIdle?.Invoke();
        PlayButtonUI.Instance.OnNextWave += () =>
        {
            if (WaveState != WaveStates.Idle) return;
            StartCoroutine(SpawnWave());
        };
        CardSelectUI.Instance.OnSelectedCard += () =>
        {
            WaveState = WaveStates.Idle;
            OnIdle?.Invoke();
        };
        waveGenerator = new WaveGenerator(waveSettings);
    }

    private void Update()
    {
        if (WaveState is WaveStates.Spawning or WaveStates.DoneSpawning)
        {
            WaveTimer += Time.deltaTime;
        }
            
    }

    private void SpawnEnemy(EnemyData enemyData) => SpawnEnemyAtPosition(enemyData, AStarPathfinding.Instance.GetPath()[0]);

    public void SpawnEnemyAtPosition(EnemyData enemyData, Vector2 position)
    {
        var enemy = Instantiate(enemyData.prefab, position, Quaternion.identity);
        enemy.GetComponent<EnemyDataHolder>().Init(enemyData);
        enemy.GetComponent<EnemyHealth>().OnDeath += () =>
        {
            OnEnemyKilled?.Invoke(enemy.GetComponent<EnemyDataHolder>().Data.coins);
            DeadEnemies.Add(enemyData);
            RemoveEnemy(enemy);
        };
        enemy.GetComponent<IMovementListener>().OnReachedEnd += () =>
        {
            OnEnemyReachedEnd?.Invoke(enemy.GetComponent<EnemyDataHolder>().Data.livesCost);
            RemoveEnemy(enemy);
        };
        currentEnemies.Add(enemy);
        OnEnemiesUpdated?.Invoke();
        enemy.GetComponent<IUsesStatusEffects>().OnEffectsUpdated += () => OnEnemiesUpdated?.Invoke();
    }

    public void SpawnEnemyBurst(EnemyData enemyData, int count, Vector2 position)
    {
        EnemyData[] dataArray = new EnemyData[count];
        for (int i = 0; i < count; i++)
        {
            dataArray[i] = enemyData;
        }

        SpawnEnemyBurst(dataArray, position);
    }
    
    public void SpawnEnemyBurst(EnemyData[] enemyData, Vector2 position)
    {
        var spawnPoints = ClusterSpawning.CreateRandomCluster(position, 4, enemyData.Length);
        for (int i = 0; i < enemyData.Length; i++)
        {
            Vector2 spawnPos = (i < spawnPoints.Length) ? spawnPoints[i] : position;
            SpawnEnemyAtPosition(enemyData.ElementAt(i), spawnPos);
        }
    }
    private void RemoveEnemy(GameObject enemy)
    {
        currentEnemies.Remove(enemy);
        OnEnemiesUpdated?.Invoke();
        if (currentEnemies.Count == 0 && WaveState == WaveStates.DoneSpawning && PlayerLife.Instance.CurrentLives > 0)
        {
            Debug.Log("Wave Complete");
            /**
            if (CurrentWave - 1 >= levelData.waves.Length)
            {
                Debug.LogError("No more waves left");
                FunctionTimer.Create(() =>
                {
                    OnNoWavesLeft?.Invoke(PlayerGameOverStats.GetGameOverData());
                }, 2f);
                return;
            }
            */
            DeadEnemies.Clear();
            WaveState = WaveStates.Complete;
            OnWaveComplete?.Invoke();
            WaveTimer = 0;
        }
    }

    private IEnumerator SpawnWave()
    {
        Debug.Log("Starting wave");
        WaveTimer = 0;
        TowerService.BeginWave(TowerDataHolder.ActiveTowerList);
        OnWaveStarted?.Invoke();
        WaveState = WaveStates.Spawning;
        var spawns = waveGenerator.Generate(CurrentWave).enemySpawns;
        foreach (var e in spawns)
        {
            SpawnEnemy(e.enemy.EnemyData);
            yield return new WaitForSeconds(e.delay);
        }
        WaveState = WaveStates.DoneSpawning;
        CurrentWave++;
    }
}