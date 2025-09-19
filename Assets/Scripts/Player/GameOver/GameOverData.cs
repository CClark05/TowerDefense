public struct GameOverData
{
    public int HighestWave { get; }
    public int EnemiesKilled { get; }
    public float MaxDps { get; }
    public int CoinsEarned { get; }

    public GameOverData(int highestWave, int enemiesKilled, float maxDps, int coinsEarned)
    {
        HighestWave = highestWave;
        EnemiesKilled = enemiesKilled;
        MaxDps = maxDps;
        CoinsEarned = coinsEarned;
    }
}