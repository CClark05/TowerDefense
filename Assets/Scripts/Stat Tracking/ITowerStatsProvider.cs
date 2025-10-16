public interface ITowerStatsProvider
{
    float MaxWaveDPS { get; }
    float AverageWaveDPS { get; }
    float TotalWaveDamage { get; }
    string ID { get; }
}
