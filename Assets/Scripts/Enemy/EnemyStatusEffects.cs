using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusEffects : MonoBehaviour, IUsesStatusEffects
{
    [SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private GridLayoutGroup layoutGroup;
    public Dictionary<PersistentStatusEffect, int> PersistentEffectsApplied { get; } = new();
    public Dictionary<PersistentStatusEffect, int> PersistentEffectsGhost { get; } = new();
    private List<EffectUI> activeEffectUI = new();
    private Dictionary<OnTickStatusEffect, TickData> OnTickEffects = new();
    public Action<DamageData, TowerShooting> OnTakeDamage { get; set; }
    public event Action OnEffectsUpdated;
    public static event Action<DamageData, TowerShooting, Vector2> OnTakeDamageStatic;
    public Dictionary<PersistentStatusEffect, int> PersistentEffectTotalApplications { get; }= new();
    private void Start()
    {
        OnTakeDamage += (data, tower) =>
        {
            OnTakeDamageStatic?.Invoke(data, tower, transform.position);
        };
    }

    public void AddPersistentEffect(PersistentStatusEffect effect, HitData hitData, int stacks, ModifyEffectData modifyData, bool ghost = false)
    {
        int newStacks = stacks + modifyData.additionalStacks;
        if (ghost)
        {
            PersistentEffectsGhost[effect] = PersistentEffectsGhost.GetValueOrDefault(effect) + newStacks;
            return;
        }
        if (effect is OnTickStatusEffect tickEffect)
        {
            if (!OnTickEffects.ContainsKey(tickEffect))
            {
                TickData tickData = new TickData(GetComponent<IDamageable>(), this, hitData.tower, hitData, tickEffect.tickInterval, newStacks);
                OnTickEffects[tickEffect] = tickData;
            }
            else
                OnTickEffects[tickEffect].stacks += newStacks;
        }
        PersistentEffectTotalApplications[effect] = PersistentEffectTotalApplications.GetValueOrDefault(effect) + 1;
        PersistentEffectsApplied[effect] = PersistentEffectsApplied.GetValueOrDefault(effect) + newStacks;
        var effectUI = activeEffectUI.Find(e => e.data == effect);
        if (effectUI != null)
        {
            effectUI.AddStacks(newStacks);
            return;
        }

        var newEffect = Instantiate(statusEffectPrefab, layoutGroup.transform).GetComponent<EffectUI>();
        newEffect.Init(effect, newStacks);
        activeEffectUI.Add(newEffect);
        OnEffectsUpdated?.Invoke();
    }
    

    public void RemoveAllStacks(PersistentStatusEffect effect, bool ghost = false)
    {
        if (ghost)
        {
            PersistentEffectsGhost.Remove(effect);
            return;
        }

        var effectUI = activeEffectUI.Find(e => e.data == effect);
        PersistentEffectsApplied.Remove(effect);
        if (effect is OnTickStatusEffect onTick)
            OnTickEffects.Remove(onTick);
        activeEffectUI.Remove(effectUI);
        Destroy(effectUI.gameObject);
        OnEffectsUpdated?.Invoke();
    }

    public void RemoveStacks(PersistentStatusEffect effect, int stacks, bool ghost = false)
    {
        if(ghost){
            if (PersistentEffectsGhost.TryGetValue(effect, out var ghostStacks))
                PersistentEffectsGhost[effect] = Mathf.Max(0, ghostStacks - stacks);
            return;
        }
        if (!PersistentEffectsApplied.TryGetValue(effect, out int current)) return;
        PersistentEffectsApplied[effect] = Mathf.Max(0, current - stacks);
        var effectUI = activeEffectUI.Find(e => e.data == effect);
        effectUI.RemoveStacks(stacks);
        if (effect is OnTickStatusEffect onTick)
            OnTickEffects[onTick].stacks = Mathf.Max(0, OnTickEffects[onTick].stacks - stacks);
        if (PersistentEffectsApplied[effect] == 0)
        {
            RemoveAllStacks(effect);
        }
        OnEffectsUpdated?.Invoke();
    }

    private void Update()
    {
        Tick();
    }

    private void Tick()
    {
        foreach (var kvp in OnTickEffects)
        {
            var tickData = kvp.Value;
            tickData.timer += Time.deltaTime;
            if (tickData.timer >= tickData.tickInterval)
            {
                tickData.timer = 0;
                kvp.Key.OnTick(tickData, tickData.stacks);
            }
        }
    }
}