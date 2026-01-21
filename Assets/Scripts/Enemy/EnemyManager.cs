using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<GameObject> currentEnemies = new();
    public List<GameObject> CurrentEnemies => currentEnemies;
    public List<EnemyData> DeadEnemiesThisWave { get; private set; } = new();
    public HashSet<EnemyData> DeadEnemies { get; private set; } = new();
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
    public WaveSettings WaveSettings => waveSettings;
    private WaveGenerator waveGenerator;
    [Header("TESTING DATA")]
    [SerializeField] private List<EnemyData> testEnemies = new();

    [SerializeField] private float delayBetweenTestSpawns = 0.5f;
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

    public HashSet<Transform> GetNearbyEnemies(Vector2 position, float range)
    {
        var nearbyEnemies = new HashSet<Transform>();
        foreach (var enemy in CurrentEnemies)
        {
            if(Vector2.Distance(enemy.transform.position, position) > range) continue;
            nearbyEnemies.Add(enemy.transform);
        }
        return nearbyEnemies;
    }

    public Transform GetClosestEnemy(Vector2 position)
    {
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (var enemy in CurrentEnemies)
        {
            float distance = Vector2.Distance(enemy.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }
    private void SpawnEnemy(EnemyData enemyData) => SpawnEnemyAtPosition(enemyData, AStarPathfinding.Instance.GetPath()[0]);

    public void SpawnEnemyAtPosition(EnemyData enemyData, Vector2 position)
    {
        var enemy = Instantiate(enemyData.prefab, position, Quaternion.identity);
        enemy.GetComponent<EnemyDataHolder>().Init(enemyData);
        enemy.GetComponent<EnemyHealth>().OnDeath += () =>
        {
            OnEnemyKilled?.Invoke(enemy.GetComponent<EnemyDataHolder>().Data.coins);
            DeadEnemiesThisWave.Add(enemyData);
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
    
    private void RemoveEnemy(GameObject enemy)
    {
        currentEnemies.Remove(enemy);
        OnEnemiesUpdated?.Invoke();
        if (currentEnemies.Count == 0 && WaveState == WaveStates.DoneSpawning && PlayerLife.Instance.CurrentLives > 0)
        {
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
            DeadEnemiesThisWave.Clear();
            WaveState = WaveStates.Complete;
            OnWaveComplete?.Invoke();
            if (EncounterGenerator.Instance.GetEncounter(CurrentWave - 1) != null)
            {
                WaveState = WaveStates.Idle;
                OnIdle?.Invoke();
            }
                
            WaveTimer = 0;
        }
    }

    private IEnumerator SpawnWave()
    {
        WaveTimer = 0;
        TowerService.BeginWave(TowerDataHolder.ActiveTowerList);
        OnWaveStarted?.Invoke();
        WaveState = WaveStates.Spawning;
        if(testEnemies.Count > 0)
        {
            foreach (var e in testEnemies)
            {
                SpawnEnemy(e);
                yield return new WaitForSeconds(delayBetweenTestSpawns);
            }
            WaveState = WaveStates.DoneSpawning;
            CurrentWave++;
            yield break;
        }
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