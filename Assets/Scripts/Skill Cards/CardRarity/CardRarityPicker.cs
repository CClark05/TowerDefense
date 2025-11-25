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

    public static List<SkillData> PickCards(List<SkillData> pool, IEnumerable<CardRarity> allowedRarities, CardRaritySettings settings, int count)
    {
        var allowed = allowedRarities?.ToHashSet()
                      ?? throw new ArgumentNullException(nameof(allowedRarities));

        var filteredChances = settings.rarityChances
            .Where(rc => allowed.Contains(rc.rarity))
            .Select(rc => new RarityChance { rarity = rc.rarity, weight = rc.weight })
            .ToList();

        if (filteredChances.Count == 0)
            throw new InvalidOperationException("No valid rarities in allowedRarities.");

        var picks = new List<SkillData>(count);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            var rarity = RollRarity(filteredChances);

            var candidates = pool.Where(s => s.rarity == rarity).ToList();
            if (candidates.Count == 0)
                candidates = pool.Where(s => allowed.Contains(s.rarity)).ToList();

            if (candidates.Count == 0)
                break;

            var choice = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            picks.Add(choice);
            pool.Remove(choice);
        }

        return picks;
    }
}