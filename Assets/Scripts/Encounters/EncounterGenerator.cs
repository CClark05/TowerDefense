using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class EncounterGenerator : Singleton<EncounterGenerator>
{
    [SerializeField] private List<EncounterSettings> encounterSettingsList;
    [SerializeField] private int waveTarget = 40;
    private int cooldownLength = 3;
    public Dictionary<int, EncounterSettings> EncounterDictionary { get; private set; }

    public EncounterSettings GetEncounter(int wave) => EncounterDictionary.GetValueOrDefault(wave);
    private new void Awake()
    {
        base.Awake();
        EncounterDictionary = GenerateEncounters();
        foreach (var kvp in EncounterDictionary)
        {
            Debug.Log($"Wave {kvp.Key}: Encounter {kvp.Value.name}");
        }
    }
    private Dictionary<int, EncounterSettings> GenerateEncounters()
    {
        var tickets = new List<EncounterSettings>();
        foreach (var e in encounterSettingsList)
        {
            int expected = Mathf.RoundToInt(e.targetRate * waveTarget);
            for (int i = 0; i < expected; i++) 
                tickets.Add(e);
        }
        for(int i = tickets.Count; i< waveTarget; i++)
            tickets.Add(null); 
        
        tickets = tickets.OrderBy(_ => UnityEngine.Random.value).ToList();
        int wave = 0;
        var result = new Dictionary<int, EncounterSettings>();
        var cooldown = new Dictionary<EncounterSettings, int>();
        var leftovers = new List<EncounterSettings>();
        while (wave < waveTarget)
        {
            var encounter = tickets[wave];
            if (encounter == null)
            {
                wave++;
                continue;
            }
            if (wave < encounter.firstWave || IsOnCooldown(encounter, wave))
            {
                leftovers.Add(encounter);
                wave++;
                continue;
            }
            result[wave] = encounter;
            cooldown[encounter] = wave + cooldownLength;
            wave++;
        }
        var emptySlots = Enumerable.Range(0, waveTarget).Where(w => !result.ContainsKey(w)).OrderBy(_ => UnityEngine.Random.value).ToList();
        leftovers = leftovers.OrderBy(_ => UnityEngine.Random.value).ToList();
        foreach (var leftover in leftovers)
        {
            foreach (var e in emptySlots)
            {
                if (leftover.firstWave > e || result.ContainsKey(e)) continue;
                result[e] = leftover;
                break;
            }
        }
        return result;
        
        bool IsOnCooldown(EncounterSettings e, int w)
        {
            return cooldown.TryGetValue(e, out var until) && w < until;
        }
    }
    
}   