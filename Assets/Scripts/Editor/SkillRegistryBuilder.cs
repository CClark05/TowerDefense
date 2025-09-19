#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using UnityEngine;

public static class SkillRegistryBuilder
{
    [MenuItem("Tools/Skills/Refresh Registry")]
    public static void Refresh()
    {
        // 1. Find all SkillData assets
        var guids = AssetDatabase.FindAssets("t:SkillData");
        var list = guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Select(p => AssetDatabase.LoadAssetAtPath<SkillData>(p))
            .Where(s => s != null)
            .Distinct()
            .ToList();

        // 2. Try to find an existing SkillRegistry anywhere in the project
        var registryGuid = AssetDatabase.FindAssets("t:SkillRegistry").FirstOrDefault();
        SkillRegistry registry = null;

        if (!string.IsNullOrEmpty(registryGuid))
        {
            var path = AssetDatabase.GUIDToAssetPath(registryGuid);
            registry = AssetDatabase.LoadAssetAtPath<SkillRegistry>(path);
        }

        // 3. If not found, create one in a default location
        if (registry == null)
        {
            registry = ScriptableObject.CreateInstance<SkillRegistry>();
            AssetDatabase.CreateAsset(registry, "Assets/SkillRegistry.asset");
        }

        // 4. Update the list
        registry.Skills = list;
        EditorUtility.SetDirty(registry);
        AssetDatabase.SaveAssets();

        Debug.Log($"SkillRegistry refreshed with {list.Count} skills");
    }
}
#endif