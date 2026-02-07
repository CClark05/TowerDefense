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
    }
    private void OnEnemyReachedEndStatic(int lives)
    {
        CurrentLives = Mathf.Max(0, CurrentLives - lives);
        VignetteController.Instance.PulseColor(Color.red, 0.4f, 0.14f);
        if (CurrentLives <= 0)
        {
            Debug.Log("GAME OVER");
            OnGameOver?.Invoke( PlayerGameOverStats.GetGameOverData());
        }
    }
    public void AddLives(int amount)
    {
        CurrentLives += amount;
    }
}
