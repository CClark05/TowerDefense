using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUsesStatusEffects
{
    public Dictionary<PersistentStatusEffect, int> PersistentEffectsApplied { get; }
    public Dictionary<PersistentStatusEffect, int> PersistentEffectTotalApplications { get; }
    public void AddPersistentEffect(PersistentStatusEffect effect, HitData hitData, int stacks, ModifyEffectData modifyData);
    public void RemoveAllStacks(PersistentStatusEffect effect);
    public void RemoveStacks(PersistentStatusEffect effect, int stacks);
    public Action<DamageData, TowerShooting> OnTakeDamage { get; set; }
    public event Action OnEffectsUpdated; 
}