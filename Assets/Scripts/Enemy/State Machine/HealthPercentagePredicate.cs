using System;
using UnityEngine;

public class HealthPercentagePredicate : IPredicate
{
    private readonly IUsesHealth uses;
    private readonly float[] milestones;   // must be descending: 0.75, 0.50, 0.25
    private int nextIdx = 0;
    private int pending = 0;       
    private float lastFrac;
    private const float Eps = 1e-4f;

    public HealthPercentagePredicate(IUsesHealth uses, params float[] milestonesDesc)
    {
        this.uses = uses;
        this.milestones = (milestonesDesc != null && milestonesDesc.Length > 0)
            ? milestonesDesc
            : new[] { 0.75f, 0.50f, 0.25f };

        var hs = uses.HealthSystem;
        lastFrac = Mathf.Clamp01((float)hs.Health / Math.Max(1, hs.MaxHealth));

        // Skip milestones already below at spawn
        while (nextIdx < milestones.Length && lastFrac <= milestones[nextIdx] + Eps)
            nextIdx++;

        hs.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(int newHealth)
    {
        var hs = uses.HealthSystem;
        float frac = Mathf.Clamp01((float)newHealth / Math.Max(1, hs.MaxHealth));

        // Only count downward crossings; support big hits crossing multiple milestones
        if (frac < lastFrac - Eps)
        {
            while (nextIdx < milestones.Length && frac <= milestones[nextIdx] + Eps)
            {
                pending++;   // enqueue one trigger per crossed milestone
                nextIdx++;
            }
        }
        lastFrac = frac;
    }
    
    public bool Evaluate()
    {
        if (pending <= 0) return false;
        pending--;
        return true;
    }

    public void Dispose()
    {
        if (uses != null) uses.HealthSystem.OnHealthChanged -= OnHealthChanged;
    }
}