using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SkillContext
{
    public List<SkillInstance> ActiveSkills { get; private set; } = new();
    public Dictionary<IBuff, int> ActiveBuffs { get; private set; } = new();
    public TowerDataHolder Tower { get; private set; } = new();
    public Action<SkillInstance> OnCardInstanceCreated;
    public event Action<TowerWaveData> OnTowerUpdated;
    public event Action<IBuff, int> OnBuffAdded;
    public event Action<IBuff, int> OnBuffRemoved;
    public event Action<SkillData> OnCardPlayed;
    public event Action<SkillData, bool> OnCardPlayTwiceUpdated;
    public SkillContext(TowerDataHolder tower)
    {
        Tower = tower;
    }
    public void AddSkill(SkillData skillData)
    {
        var instance = skillData.CreateInstance();
        instance.OnPlayTwiceUpdated += (playTwice) => OnCardPlayTwiceUpdated?.Invoke(skillData, playTwice);
        ActiveSkills.Add(instance);
        instance.SetContext(this);
        TowerWaveData towerWaveData = new TowerWaveData();
        OnCardInstanceCreated?.Invoke(instance);
        if (TowerService.TryModifyOnCardReceived(towerWaveData, instance))
            OnTowerUpdated?.Invoke(towerWaveData);
        instance.OnPlayCard += () => OnCardPlayed?.Invoke(skillData);
        
    }
    public bool TryRemoveSkill(SkillData skillData)
    {
        var instance = ActiveSkills.LastOrDefault(s => s.Data == skillData);
        if (instance == null) return false;
        instance.Dispose();
        ActiveSkills.Remove(instance);
        var towerWaveData = new TowerWaveData();
        if (TowerService.TryModifyOnCardRemoved(towerWaveData, instance))
            OnTowerUpdated?.Invoke(towerWaveData);
        return true;
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
