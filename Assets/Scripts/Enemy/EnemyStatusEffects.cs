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

    public void AddPersistentEffect(PersistentStatusEffect effect, HitData hitData, int stacks, ModifyEffectData modifyData)
    {
        int newStacks = stacks + modifyData.additionalStacks;
        int? newDuration = modifyData.duration;
        if (effect is OnTickStatusEffect tickEffect)
        {
            if (!OnTickEffects.TryGetValue(tickEffect, out var tickData))
            {
                tickData = new TickData(GetComponent<IDamageable>(), this, hitData.tower, hitData, tickEffect.tickInterval);
                OnTickEffects[tickEffect] = tickData;
            }
            var skillContext = hitData.dataHolder.SkillContext;
            
            if (!tickData.stacks.TryGetValue(skillContext, out var bucket))
            {
                bucket = new TickData.Stack(newDuration, newStacks);
                tickData.stacks[skillContext] = bucket;
            }
            else
                bucket.Add(newStacks, newDuration);
        }
        PersistentEffectTotalApplications[effect] = PersistentEffectTotalApplications.GetValueOrDefault(effect) + 1;
        PersistentEffectsApplied[effect] = PersistentEffectsApplied.GetValueOrDefault(effect) + newStacks;
        var effectUI = activeEffectUI.Find(e => e.data == effect);
        if (effectUI != null)
        {
            effectUI.AddStacks(newStacks);
            OnEffectsUpdated?.Invoke();
            return;
        }
        var newEffect = Instantiate(statusEffectPrefab, layoutGroup.transform).GetComponent<EffectUI>();
        newEffect.Init(effect, newStacks);
        activeEffectUI.Add(newEffect);
        OnEffectsUpdated?.Invoke();
    }



    public void RemoveAllStacks(PersistentStatusEffect effect)
    {
        var effectUI = activeEffectUI.Find(e => e.data == effect);
        PersistentEffectsApplied.Remove(effect);
        if (effect is OnTickStatusEffect onTick)
            OnTickEffects.Remove(onTick);
        activeEffectUI.Remove(effectUI);
        Destroy(effectUI.gameObject);
        OnEffectsUpdated?.Invoke();
    }

    public void RemoveStacks(PersistentStatusEffect effect, int stacks)
    {
        if (!PersistentEffectsApplied.TryGetValue(effect, out int current)) return;
        PersistentEffectsApplied[effect] = Mathf.Max(0, current - stacks);
        var effectUI = activeEffectUI.Find(e => e.data == effect);
        effectUI.RemoveStacks(stacks);
        /**
        if (effect is OnTickStatusEffect onTick)
            OnTickEffects[onTick].stacks = Mathf.Max(0, OnTickEffects[onTick].stacks - stacks);
            */
        if (PersistentEffectsApplied[effect] == 0)
        {
            RemoveAllStacks(effect);
        }
        OnEffectsUpdated?.Invoke();
    }

    private void RemoveTickEffect(PersistentStatusEffect effect, TickData tickData, int stacks, SkillContext skillContext)
    {
        if(effect is not OnTickStatusEffect) return;
        if (!PersistentEffectsApplied.TryGetValue(effect, out int current)) return;
        
        foreach (var kvp in OnTickEffects.ToList())
        {
            if(kvp.Value.stacks.TryGetValue(skillContext, out TickData.Stack stack) && kvp.Key == effect)
            {
                PersistentEffectsApplied[effect] = Mathf.Max(0, current - stacks);
                var effectUI = activeEffectUI.Find(e => e.data == effect);
                effectUI.RemoveStacks(stacks);
                kvp.Value.stacks[skillContext].Count = Mathf.Max(0, stack.Count - stacks);
                if (kvp.Value.TotalStacks <= 0)
                    RemoveAllStacks(effect);
                OnEffectsUpdated?.Invoke();
            }
        }
        
    }
    private void Update()
    {
        Tick();
    }

    private void Tick()
    {
        if (OnTickEffects.Count == 0) return;
        /**
        foreach (var kvp in OnTickEffects.ToList())
        {
            var tickData = kvp.Value;
            tickData.timer += Time.deltaTime;
            if (tickData.timer >= tickData.tickInterval)
            {
                tickData.timer = 0;
                tickData.durationElapsed += tickData.tickInterval;
                kvp.Key.OnTick(tickData);
                foreach (var _kvp in tickData.stacks.ToList())
                {
                    if (_kvp.Value.Duration.HasValue && tickData.durationElapsed >= _kvp.Value.Duration.Value)
                    {
                        RemoveTickEffect(kvp.Key, tickData, 1, _kvp.Key);
                    }
                }
            }
        }
        */
        
        foreach (var kvp in OnTickEffects.ToList())
        {
            var effect = kvp.Key;
            var tickData = kvp.Value;

            tickData.timer += Time.deltaTime;
            if (tickData.timer < tickData.tickInterval) 
                continue;

            tickData.timer -= tickData.tickInterval;
            
            effect.OnTick(tickData);
            float now = Time.time;
            foreach (var ctx in tickData.stacks.Keys.ToList())
            {
                var stack = tickData.stacks[ctx];
                int expired = stack.ExpireNow(now);
                if (expired > 0)
                    RemoveStacks(effect, expired);
                
                if (stack.Count <= 0)
                    tickData.stacks.Remove(ctx);
            }
            /**
            if (tickData.TotalStacks <= 0)
                RemoveAllStacks(effect);
                */
        }
    }
}