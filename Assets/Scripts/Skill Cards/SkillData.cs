using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum CardRarity
{
    Common,
    Rare,
    Legendary,
}
[System.Serializable]
public class EffectEntry
{
    public bool showInUI = true;
    public EffectData data;
}
public abstract class SkillData : ScriptableObject
{
    public int price => GetPriceByRarity(rarity);

    private int GetPriceByRarity(CardRarity cardRarity)
    {
        switch (cardRarity)
        {
            case CardRarity.Common:
                return 25;
            case CardRarity.Rare:
                return 50;
            case CardRarity.Legendary:
                return 125;
            default:
                throw new ArgumentOutOfRangeException(nameof(cardRarity), cardRarity, null);
        }
    }
    
    public string name;
    public Sprite icon;
    public CardRarity rarity;
    [TextArea] public string description;
    public EffectEntry[] statusEffects;
    public EffectEntry[] buffs;
    public SkillData[] prerequisiteSkills;
    public abstract SkillInstance CreateInstance();
}

