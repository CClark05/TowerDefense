using UnityEngine;

public static class SOExtensions
{
    public static T CloneRuntime<T>(this T asset) where T : ScriptableObject
    {
        var inst = ScriptableObject.Instantiate(asset);
        inst.name = asset.name + " (Runtime)";
        inst.hideFlags = HideFlags.DontSave; 
        return inst;
    }
}