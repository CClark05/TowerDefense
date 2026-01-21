using UnityEngine;

[System.Serializable]
public class BudgetTuning
{
    public float DRef = 18f;         // reference damage per hit constant
    public float SRef = 4f;       // reference speed constant 
    public float ScaleK = 0.10f;    // global scale to budget points
    public float MinSpeedMult = 0.6f;
    public float MaxSpeedMult = 2f;
    public int   MinCost = 1; // floor
    public int EliteBonusPercent = 50; // percent increase for elites
}

public static class EnemyBudget
{
    public static int Cost(int hp, int shields, float speed, BudgetTuning t, bool isElite)
    {
        float effectiveHp = hp + shields * t.DRef;
        float speedMult = Mathf.Clamp(speed / t.SRef, t.MinSpeedMult, t.MaxSpeedMult);

        int cost = Mathf.Max(t.MinCost, Mathf.RoundToInt(t.ScaleK * effectiveHp * speedMult));
        return cost * (isElite ? t.EliteBonusPercent : 1);
    }
}
