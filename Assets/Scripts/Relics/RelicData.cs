using UnityEngine;

[CreateAssetMenu(fileName = "Relic Data", menuName = "Relics")]
public abstract class RelicData : ScriptableObject
{
    public string name;
    public Sprite sprite;
    [TextArea] public string description;

    public abstract RelicInstance CreateInstance();
}