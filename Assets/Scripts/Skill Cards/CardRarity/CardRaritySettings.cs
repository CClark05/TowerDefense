using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Rarity Settings", menuName = "SkillData/RaritySettings")]
public class CardRaritySettings : ScriptableObject
{
    public List<RarityChance> rarityChances = new()
    {
        new RarityChance { rarity = CardRarity.Common, weight = 70 },
        new RarityChance { rarity = CardRarity.Rare, weight = 25 },
        new RarityChance { rarity = CardRarity.Legendary, weight = 5 },
    };
}
[System.Serializable]
public struct RarityChance
{
    public CardRarity rarity;
    public float weight; 
}