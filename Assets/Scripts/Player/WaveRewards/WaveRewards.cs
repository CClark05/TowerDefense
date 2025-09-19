using UnityEngine;

public readonly struct WaveRewards
{
    public int InitialCoins { get; }
    public int BaseReward { get; }
    public int LeftoverMoves { get; }
    public float InterestRate { get; }
    public int CoinsPerMove { get; }

    public int InterestAmount => Mathf.FloorToInt(InitialCoins * InterestRate);
    public int LeftoverMovesReward => LeftoverMoves * CoinsPerMove;
    public int TotalReward => InterestAmount + LeftoverMovesReward + BaseReward;
    public int WaveNumber { get; }

    public WaveRewards(float interestRate, int coinsPerMove, int initialCoins, int baseReward, int leftoverMoves, int waveNumber)
    {
        InterestRate = interestRate;
        CoinsPerMove = coinsPerMove;
        InitialCoins = initialCoins;
        BaseReward = baseReward;
        LeftoverMoves = leftoverMoves;
        WaveNumber = waveNumber;
    }
}