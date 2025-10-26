using System;
using System.Collections.Generic;
using System.Linq;

public static class CardRarityPicker
{
    private static CardRarity RollRarity(List<RarityChance> chances)
    {
        float total = chances.Sum(c => c.weight);
        float roll = UnityEngine.Random.Range(0f, total);

        float accum = 0;
        foreach (var c in chances)
        {
            accum += c.weight;
            if (roll <= accum)
                return c.rarity;
        }

        return chances[0].rarity;
    }

    public static List<SkillData> PickCards(List<SkillData> pool, CardRaritySettings settings, int count) =>
        PickCards(pool, settings, count, null);

    public static List<SkillData> PickCards(List<SkillData> pool, CardRaritySettings settings, int count, Func<CardRarity, float> weightMod)
    {
        var picks = new List<SkillData>(count);
        var weighted = settings.rarityChances
            .Select(rc => new RarityChance
            {
                rarity = rc.rarity,
                weight = rc.weight * (weightMod?.Invoke(rc.rarity) ?? 1f)
            })
            .ToList();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            var rarity = RollRarity(weighted);
            
            var candidates = pool.Where(s => s.rarity == rarity).ToList();
            if (candidates.Count == 0) candidates = pool;

            var choice = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            picks.Add(choice);
            pool.Remove(choice);
        }

        return picks;
    }
}