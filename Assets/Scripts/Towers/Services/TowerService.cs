
using System.Collections.Generic;
using UnityEngine;

public static class TowerService
{
    private static readonly Dictionary<TowerDataHolder, TowerWaveData> waveData = new();
    private static readonly List<BorrowRequest> requests = new();
    private static int expectedTowers;
    private static int finishedTowers;
    public static void BeginWave(IReadOnlyList<TowerDataHolder> towers)
    {
        waveData.Clear();
        requests.Clear();
        expectedTowers = towers.Count;
        finishedTowers = 0;
        
        foreach (var t in towers)
        {
            t.WaveData = new TowerWaveData
            {
                owner = t,
                startingSnapshot = new List<SkillData>(t.SkillDataList)
            };
            waveData[t] = t.WaveData;
        }
    }
    public static void MarkWaveStartDone(TowerDataHolder tower)
    {
        finishedTowers++;
        if (finishedTowers == expectedTowers)
        {
            foreach (var wd in waveData.Values)
            {
                foreach (var req in wd.borrowRequests)
                {
                    if (req.lender == null || req.card == null) continue;
                    if (req.lender.TryRemoveCard(req.card))
                    {
                        req.fulfilled = true;
                        req.borrower.AddCard(req.card);
                    }
                }
            }
            foreach (var kv in waveData)
                kv.Key.UpdateTowerData(kv.Value);
        }
    }
    
    public static void ModifyWaveStart(TowerWaveData data, SkillContext context)
    {
        foreach (var mod in context.GetSkillInstancesWith<ITowerWaveStartModifier>())
        {
            mod.modifier.Modify(data);
            if (mod.instance.PlayTwice)
                mod.modifier.Modify(data);
        }
    }

    public static void ModifyWaveEnd(TowerWaveData data, SkillContext context)
    {
        foreach (var mod in context.GetSkillInstancesWith<ITowerWaveEndModifier>())
        {
            mod.modifier.Modify(data);
            if (mod.instance.PlayTwice)
                mod.modifier.Modify(data);
        }
    }

    public static bool TryModifyOnCardReceived(TowerWaveData data, SkillInstance skillInstance)
    {
        if (skillInstance is ITowerCardReceivedModifier modifier)
        {
            modifier.Apply(data);
            if(skillInstance.PlayTwice)
                modifier.Apply(data);
            return true;
        }
        return false;
    }
    
    public static bool TryModifyOnCardRemoved(TowerWaveData data, SkillInstance skillInstance)
    {
        if (skillInstance is ITowerCardReceivedModifier modifier)
        {
            modifier.Remove(data);
            if(skillInstance.PlayTwice)
                modifier.Remove(data);
            return true;
        }
        return false;
    }

    public static void ApplyBuff(IBuff buff, TowerWaveData data, int stacks)
    {
        for(int i = 0; i<stacks; i++)
            buff.Apply(data);
    }
    public static void RemoveBuff(IBuff buff, TowerWaveData data, int stacks)
    {
        for (int i = 0; i < stacks; i++)
            buff.Remove(data);
    }
}