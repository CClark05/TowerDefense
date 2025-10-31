using System.Collections;
using UnityEngine;

public class GainShields : EnemyCommand
{
    private int shieldAmount;
    public GainShields(int shieldAmount)
    {
        this.shieldAmount = shieldAmount;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        var shields = agent.Require<IUsesShields>();
        shields.AddShields(shieldAmount);
        yield break;
    }
}