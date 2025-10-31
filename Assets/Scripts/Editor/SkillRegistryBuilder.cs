#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using UnityEngine;

public static class SkillRegistryBuilder
{
    [MenuItem("Tools/Skills/Refresh Registry")]
    [MenuItem("Tools/Skills/Refresh Registry")]
    public static void Refresh()
    {
        // 1. Find all SkillData assets
        var guids = AssetDatabase.FindAssets("t:SkillData");
        var allSkills = guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Select(p => AssetDatabase.LoadAssetAtPath<SkillData>(p))
            .Where(s => s != null)
            .Distinct()
            .ToList();

        // 2. Filter out excluded ones
        var includedSkills = allSkills
            .Where(s => s.isExcluded == false)   // <- new filter
            .ToList();

        // 3. Try to find an existing SkillRegistry anywhere in the project
        var registryGuid = AssetDatabase.FindAssets("t:SkillRegistry").FirstOrDefault();
        SkillRegistry registry = null;

        if (!string.IsNullOrEmpty(registryGuid))
        {
            var path = AssetDatabase.GUIDToAssetPath(registryGuid);
            registry = AssetDatabase.LoadAssetAtPath<SkillRegistry>(path);
        }

        // 4. If not found, create one in a default location
        if (registry == null)
        {
            registry = ScriptableObject.CreateInstance<SkillRegistry>();
            AssetDatabase.CreateAsset(registry, "Assets/SkillRegistry.asset");
        }

        // 5. Also clean anything that's already in the registry but is now excluded
        //    (in case you toggled the bool after the last refresh)
        //    We basically just overwrite with the filtered list.
        registry.Skills = includedSkills;

        EditorUtility.SetDirty(registry);
        AssetDatabase.SaveAssets();

        Debug.Log($"SkillRegistry refreshed with {includedSkills.Count} skills (excluded: {allSkills.Count - includedSkills.Count})");
    }
}
#endif