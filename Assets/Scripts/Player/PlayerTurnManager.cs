using System;
using UnityEngine;

public class PlayerTurnManager : Singleton<PlayerTurnManager>
{
    private int movesRemaining;
    public int MovesRemaining => movesRemaining;
    public event Action<int> OnMovesUpdated;

    private void Awake()
    {
        base.Awake();
        movesRemaining = LevelDataHolder.Instance.Data.waves[0].movesAllowed;
    }

    private void Start()
    {
        GetComponent<PlayerMovement>().OnMove += LoseTurn;
        GetComponent<PlayerGathering>().OnGather += LoseTurn;
        BuildingManager.Instance.OnPlacedBuilding += i => LoseTurn();
        EnemyManager.Instance.OnIdle += () =>
        {
            movesRemaining += LevelDataHolder.Instance.Data.waves[EnemyManager.Instance.CurrentWave - 1].movesAllowed;
            OnMovesUpdated?.Invoke(movesRemaining);
        };
        //TowerSelectUI.Instance.OnSellTower += i => LoseTurn();
    }

    private void LoseTurn()
    {
        movesRemaining--;
        OnMovesUpdated?.Invoke(movesRemaining);
    }
}
