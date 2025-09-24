using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<GameObject> currentEnemies = new();
    public List<GameObject> CurrentEnemies => currentEnemies;
    private LevelData levelData;
    public int CurrentWave { get; private set; } = 1;

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
    public event Action<int> OnWaveComplete;
    public event Action<GameOverData> OnNoWavesLeft;
    public event Action OnEnemiesUpdated;
    private WaveData waveData => levelData.waves[CurrentWave - 1];

    private void Start()
    {
        levelData = LevelDataHolder.Instance.Data;
        OnIdle?.Invoke();
        PlayButtonUI.Instance.OnNextWave += () =>
        {
            if (WaveState != WaveStates.Idle) return;
            StartCoroutine(SpawnWave(waveData));
        };
        CardSelectUI.Instance.OnSelectedCard += () =>
        {
            WaveState = WaveStates.Idle;
            OnIdle?.Invoke();
        };
    }

    private void SpawnEnemy(EnemyData enemyData) => SpawnEnemyAtPosition(enemyData, AStarPathfinding.Instance.GetPath()[0]);

    public void SpawnEnemyAtPosition(EnemyData enemyData, Vector2 position)
    {
        var enemy = Instantiate(enemyData.prefab, position, Quaternion.identity);
        enemy.GetComponent<EnemyDataHolder>().Init(enemyData);
        enemy.GetComponent<EnemyHealth>().OnDeath += () => RemoveEnemy(enemy);
        enemy.GetComponent<EnemyMovement>().OnReachedEnd += () => RemoveEnemy(enemy);
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
    
    private void SpawnEnemyBurst(EnemyData[] enemyData, Vector2 position)
    {
        var spawnPoints = ClusterSpawning.CreateRandomCluster(position, 3, enemyData.Length);
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
            if (CurrentWave - 1 >= levelData.waves.Length)
            {
                Debug.LogError("No more waves left");
                OnNoWavesLeft?.Invoke(PlayerGameOverStats.GetGameOverData());
                return;
            }

            WaveState = WaveStates.Complete;
            OnWaveComplete?.Invoke(waveData.reward);
        }
    }

    private IEnumerator SpawnWave(WaveData data)
    {
        Debug.Log("Starting wave");
        OnWaveStarted?.Invoke();
        WaveState = WaveStates.Spawning;
        foreach (var enemy in data.enemies)
        {
            SpawnEnemy(enemy);
            yield return new WaitForSeconds(data.delayBetweenSpawns);
        }

        WaveState = WaveStates.DoneSpawning;
        CurrentWave++;
    }
}