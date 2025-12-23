using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerLife : Singleton<PlayerLife>
{
    private int currentLives;
    public int CurrentLives
    {
        get => currentLives;
        set
        {
            if (Equals(value, currentLives)) return;
            currentLives = value;
            Debug.Log("Lives Updated: " + currentLives);
            foreach (var tower in TowerDataHolder.ActiveTowerList)
            {
                CallModifier.Call<IOnLivesUpdated>(tower.SkillContext, (mod,instance) => mod.OnLivesUpdated(currentLives));
            }
            if (currentLives == value)
                OnLivesUpdated?.Invoke(currentLives);
        }
    }
    public event Action<int> OnLivesUpdated;
    public event Action<GameOverData> OnGameOver;
    private void Start()
    {
        CurrentLives = LevelDataHolder.Instance.Data.playerLives;
        EnemyManager.Instance.OnEnemyReachedEnd += OnEnemyReachedEndStatic;
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            if (currentLives <= 0)
            {
                Debug.Log("GAME OVER");
                OnGameOver?.Invoke( PlayerGameOverStats.GetGameOverData());
            }
        };
    }
    private void OnEnemyReachedEndStatic(int lives)
    {
        if (CurrentLives <= 0) return;
        CurrentLives -= lives;
    }
    public void AddLives(int amount)
    {
        CurrentLives += amount;
    }
}
