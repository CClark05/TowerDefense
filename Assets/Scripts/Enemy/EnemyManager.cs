using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private List<GameObject> currentEnemies = new();
    public List<GameObject> CurrentEnemies => currentEnemies;
    private LevelData levelData;
    public int CurrentWave { get; private set; } = 1;
    public enum WaveStates {Idle,Spawning, DoneSpawning, Complete}

    public WaveStates WaveState { get; private set; } = WaveStates.Idle;
    
    public event Action OnIdle;
    public event Action OnWaveStarted;
    public event Action<int> OnWaveComplete;
    public event Action<GameOverData> OnNoWavesLeft;
    public event Action OnEnemiesUpdated;
    private void Start()
    {
        levelData = LevelDataHolder.Instance.Data;
        OnIdle?.Invoke();
        PlayButtonUI.Instance.OnNextWave += () =>
        {
            if (WaveState != WaveStates.Idle) return;
            StartCoroutine(SpawnWave(levelData.waves[CurrentWave - 1]));
        };
        CardSelectUI.Instance.OnSelectedCard += () =>
        {
            WaveState = WaveStates.Idle;
            OnIdle?.Invoke();
        };
    }

    private void SpawnEnemy(EnemyData enemyData, WaveData waveData)
    {
        var enemy = Instantiate(enemyData.prefab, AStarPathfinding.Instance.GetPath()[0], Quaternion.identity);
        enemy.GetComponent<EnemyDataHolder>().Init(enemyData);
        enemy.GetComponent<EnemyHealth>().OnDeath += () => RemoveEnemy(enemy, waveData);
        enemy.GetComponent<EnemyMovement>().OnReachedEnd += () => RemoveEnemy(enemy, waveData);
        currentEnemies.Add(enemy);
        OnEnemiesUpdated?.Invoke();
        enemy.GetComponent<IUsesStatusEffects>().OnEffectsUpdated += () => OnEnemiesUpdated?.Invoke();
    }

    private void RemoveEnemy(GameObject enemy, WaveData waveData)
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
            SpawnEnemy(enemy, data);
            yield return new WaitForSeconds(data.delayBetweenSpawns);
        }

        WaveState = WaveStates.DoneSpawning;
        CurrentWave++;
    }
}
