using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EffectData : ScriptableObject
{
    public string name;
    [TextArea]
    public string description;
    public Color color;
    public Sprite sprite;
    public virtual void Execute(HitData hitData)
    {
    }
}
public abstract class StatusEffectData : EffectData
{
    public static readonly Dictionary<StatusEffectData, string> ColorHexMap = new();
    public float damageMarkerSizeMult = 1;
    public bool damageMarkerPunchEffect;
    protected virtual void OnEnable()
    {
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color)}";
        if (!ColorHexMap.ContainsKey(this))
            ColorHexMap[this] = hex;
        else
            ColorHexMap[this] = hex; 
    }
}