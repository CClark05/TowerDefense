using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUsesStatusEffects
{
    public Dictionary<PersistentStatusEffect, int> PersistentEffectsApplied { get; }
    public Dictionary<PersistentStatusEffect, int> PersistentEffectsGhost { get; }
    public Dictionary<PersistentStatusEffect, int> PersistentEffectTotalApplications { get; }
    public void AddPersistentEffect(PersistentStatusEffect effect, HitData hitData, int stacks, ModifyEffectData modifyData, bool ghost = false);
    public void RemoveAllStacks(PersistentStatusEffect effect, bool ghost = false);
    public void RemoveStacks(PersistentStatusEffect effect, int stacks, bool ghost = false);
    public Action<DamageData, TowerShooting> OnTakeDamage { get; set; }
    public event Action OnEffectsUpdated; 
}