using System.Collections.Generic;
using System.Linq;

public static class CardRarityPicker
{
    public static CardRarity RollRarity(List<RarityChance> chances)
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
    
    public static List<SkillData> PickCards(List<SkillData> pool, CardRaritySettings settings, int count)
    {
        List<SkillData> picks = new();

        for (int i = 0; i < count; i++)
        {
            var rarity = CardRarityPicker.RollRarity(settings.rarityChances);
            var candidates = pool.Where(s => s.rarity == rarity).ToList();
            if (candidates.Count == 0)
                candidates = pool;
            var choice = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            picks.Add(choice);
            pool.Remove(choice);
        }

        return picks;
    }
}
