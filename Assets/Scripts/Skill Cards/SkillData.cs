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
    public int price;
    public string name;
    public Sprite icon;
    public CardRarity rarity;
    [TextArea] public string description;
    public EffectEntry[] statusEffects;
    public EffectEntry[] buffs;
    public SkillData[] prerequisiteSkills;
    public abstract SkillInstance CreateInstance();
}

