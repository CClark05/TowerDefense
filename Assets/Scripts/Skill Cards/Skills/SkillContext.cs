using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillContext
{
    public List<SkillInstance> ActiveSkills { get; private set; } = new();
    public Dictionary<IBuff, int> ActiveBuffs { get; private set; } = new();
    public TowerDataHolder Tower { get; private set; }
    public event Action<TowerWaveData> OnTowerUpdated;
    public event Action<IBuff, int> OnBuffAdded;
    public event Action<IBuff, int> OnBuffRemoved;
    public event Action<SkillInstance> OnCardPlayed;
    public SkillContext(TowerDataHolder tower)
    {
        Tower = tower;
    }
    public void AddSkill(SkillInstance instance, int? playCount)
    {
        TowerWaveData towerWaveData = new TowerWaveData();
        ActiveSkills.Add(instance);
        instance.SetContext(this);
        if (playCount.HasValue)
            instance.PlayCount = playCount.Value;
        CallModifier.Call<IOnNewCardAdded>(this, (mod, _instance) =>
        {
            if (_instance == instance) return;
            mod.Modify(instance, towerWaveData);
        });
        /**
        foreach(var mod in GetSkillInstancesWith<IOnNewCardAdded>())
        {
            if(mod.instance == instance) continue;
            for(int i = 0; i< mod.instance.PlayCount; i++)
                mod.modifier.Modify(instance, towerWaveData);
        }
        */
        if (instance is ISelfDestructs selfDestructs)
        {
            selfDestructs.OnSelfDestruct += OnSelfDestruct;
            void OnSelfDestruct()
            {
                Debug.Log("Applying OnCardSelfDestruct modifier");
                var towerWaveData = new TowerWaveData();
                CallModifier.Call<IOnCardSelfDestruct>(this, (mod, _) =>
                {
                    mod.Apply(towerWaveData);
                });
                /**
                foreach (var mod in GetSkillInstancesWith<IOnCardSelfDestruct>())
                {
                    for (int i = 0; i < mod.instance.PlayCount; i++)
                    {
                        mod.modifier.Apply(towerWaveData);
                    }
                    mod.modifier.OnComplete();
                }
                */
                instance.Dispose();
                selfDestructs.OnSelfDestruct = null;
                OnTowerUpdated?.Invoke(towerWaveData);
            }
            
        }
        TowerService.TryModifyOnCardReceived(towerWaveData, instance);
        OnTowerUpdated?.Invoke(towerWaveData);
        instance.OnPlayCard += () => OnCardPlayed?.Invoke(instance);
        /**
        instance.OnUpdateTower += (data) =>
        {
            if (instance.skillContext != this)
            {
                Debug.LogError("SkillInstance's context does not match SkillContext in OnUpdateTower");
                return;
            }
            Debug.Log("test " + instance.Data.name);
            OnTowerUpdated?.Invoke(data);
        };
        */

    }
    public bool TryRemoveSkill(SkillInstance instance)
    {
        bool activeInstance = ActiveSkills.Any(s => s == instance);
        if (!activeInstance) return false;
        var towerWaveData = new TowerWaveData();
        if(TowerService.TryModifyOnCardRemoved(towerWaveData, instance))
            OnTowerUpdated?.Invoke(towerWaveData);
        ActiveSkills.Remove(instance);
        instance.SetContext(null);
        if(instance.IsDisabled) 
            instance.IsDisabled = false;
        //instance.Dispose();
        return true;
    }

    public void DisableCard(SkillInstance instance)
    {
        var towerWaveData = new TowerWaveData();
        if (TowerService.TryModifyOnCardRemoved(towerWaveData, instance, true))
        {
            Debug.Log("Removed card effects for: " + instance.Data.name);
            OnTowerUpdated?.Invoke(towerWaveData);
        }
    }
    public void EnableCard(SkillInstance instance)
    {
        var towerWaveData = new TowerWaveData();
        TowerService.TryModifyOnCardReceived(towerWaveData, instance);
        OnTowerUpdated?.Invoke(towerWaveData);
    }
    public IEnumerable<T> GetSkillsOfType<T>()
    {
        return ActiveSkills.OfType<T>();
    }

    public bool HasSkill<T>() where T : SkillInstance
    {
        return ActiveSkills.Any(s => s is T);
    }

    public SkillInstance GetSkill(System.Type type)
    {
        return ActiveSkills.FirstOrDefault(s => s.GetType() == type);
    }
    public IEnumerable<(SkillInstance instance, T modifier)> GetSkillInstancesWith<T>() where T : class
        => ActiveSkills.ToArray()                                
            .Select(i => (i, i as T))
            .Where(t => t.Item2 != null)!                        
            .ToArray();  
    

    public void AddBuff(IBuff buff, int stacks)
    {
        ActiveBuffs[buff] = ActiveBuffs.GetValueOrDefault(buff) + stacks;
        var towerWaveData = new TowerWaveData();
        TowerService.ApplyBuff(buff, towerWaveData, stacks);
        OnTowerUpdated?.Invoke(towerWaveData);
        OnBuffAdded?.Invoke(buff, stacks);
    }

    public bool TryRemoveBuff(IBuff buff, int stacks)
    {
        if (!ActiveBuffs.TryGetValue(buff, out int currentStacks)) return false;
        ActiveBuffs[buff] = Mathf.Max(0, currentStacks - stacks);
        var towerWaveData = new TowerWaveData();
        TowerService.RemoveBuff(buff, towerWaveData, stacks);
        OnTowerUpdated?.Invoke(towerWaveData);
        OnBuffRemoved?.Invoke(buff, stacks);
        if (ActiveBuffs[buff] == 0)
            ActiveBuffs.Remove(buff);
        return true;
    }
    
    
}
