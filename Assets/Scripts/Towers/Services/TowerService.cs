
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TowerService
{
    private static readonly Dictionary<TowerDataHolder, TowerWaveData> waveData = new();
    private static int expectedTowers;
    private static int finishedTowers;
    private static List<BorrowRequest> fulfilledRequests = new();
    public static void BeginWave(IReadOnlyList<TowerDataHolder> towers)
    {
        waveData.Clear();
        fulfilledRequests.Clear();
        expectedTowers = towers.Count;
        finishedTowers = 0;
        
        foreach (var t in towers)
        {
            t.WaveData = new TowerWaveData
            {
                owner = t,
                startingSnapshot = new List<SkillInstance>(t.SkillInstanceList)
            };
            waveData[t] = t.WaveData;
        }
    }
    public static void MarkWaveStartDone(TowerDataHolder tower)
    {
        finishedTowers++;
        if (finishedTowers == expectedTowers)
        {
            var requests = waveData.Values.SelectMany(wd => wd.borrowRequests)
                .Where(r => r.borrower != null && r.lender != null && r.card != null)
                .ToList();
            //Requests between the same two towers 
            var pairs = requests.GroupBy(r =>
            {
                var a = r.borrower.GetInstanceID();
                var b = r.lender.GetInstanceID();
                return (min: Math.Min(a, b), max: Math.Max(a, b));
            });
            foreach (var pair in pairs)
            {
                var sample = pair.First();
                TowerDataHolder A = (sample.borrower.GetInstanceID() < sample.lender.GetInstanceID()) ? sample.borrower : sample.lender;
                TowerDataHolder B = (A == sample.borrower) ? sample.lender : sample.borrower;
                var AtoBRequests = pair.Where(r => r.borrower == B && r.lender == A).ToList(); 
                var BtoARequests = pair.Where(r => r.borrower == A && r.lender == B).ToList();
                int n = Mathf.Min(AtoBRequests.Count, BtoARequests.Count);
                for(int i = 0; i<n; i++)
                {
                    var AB = AtoBRequests[i]; // A -> B
                    var BA = BtoARequests[i]; // B -> A
                    BA.fulfilled = B.TryRemoveCard(BA.card);
                    AB.fulfilled = A.TryRemoveCard(AB.card);
                    if (AB.fulfilled)
                    {
                        B.AddCard(AB.card, AB.playCount);
                        fulfilledRequests.Add(AB);
                    }
                    if (BA.fulfilled)
                    {
                        A.AddCard(BA.card, BA.playCount);
                        fulfilledRequests.Add(BA);
                    }
                    
                }
                for(int i = n; i<AtoBRequests.Count; i++)
                {
                    var AB = AtoBRequests[i];
                    AB.fulfilled = A.TryRemoveCard(AB.card);
                    if (AB.fulfilled)
                    {
                        B.AddCard(AB.card, AB.playCount);
                        fulfilledRequests.Add(AB);
                    }
                        
                }
                for(int i = n; i<BtoARequests.Count; i++)
                {
                    var BA = BtoARequests[i];
                    BA.fulfilled = B.TryRemoveCard(BA.card);
                    if (BA.fulfilled)
                    {
                        A.AddCard(BA.card, BA.playCount);
                        fulfilledRequests.Add(BA);
                    }
                        
                }
            }
            foreach (var kv in waveData)
                kv.Key.UpdateTowerData(kv.Value);
            finishedTowers = 0;
        }
        
    }
    public static void MarkWaveEnd(TowerDataHolder tower)
    {
        finishedTowers++;
        if (finishedTowers == expectedTowers)
        {
            foreach (var request in fulfilledRequests)
            {
                if(!request.borrower.TryRemoveCard(request.card))
                    Debug.LogError("Failed to remove borrowed card at wave end");
            }
            foreach(var request in fulfilledRequests)
            {
                request.lender.AddCard(request.card);
            }
        }
    }
    
    public static void ModifyWaveStart(TowerWaveData data, SkillContext context)
    {
        foreach (var mod in context.GetSkillInstancesWith<ITowerWaveStartModifier>())
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.Modify(data);
            }
        }
    }

    public static void ModifyWaveEnd(TowerWaveData data, SkillContext context)
    {
        foreach (var mod in context.GetSkillInstancesWith<ITowerWaveEndModifier>())
        {
            for (int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.Modify(data);
            }
        }
    }

    public static bool TryModifyOnCardReceived(TowerWaveData data, SkillInstance skillInstance)
    {
        if (skillInstance is ITowerCardReceivedModifier modifier)
        {
            for (int i = 0; i < skillInstance.PlayCount; i++)
            {
                modifier.Apply(data);
            }
            return true;
        }
        return false;
    }
    
    public static bool TryModifyOnCardRemoved(TowerWaveData data, SkillInstance skillInstance)
    {
        if (skillInstance is ITowerCardReceivedModifier modifier)
        {
            for (int i = 0; i < skillInstance.PlayCount; i++)
            {
                modifier.Remove(data);
            }
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