using UnityEditor;

[CustomEditor(typeof(SkillData), true)]
public class SkillDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Pull properties
        SerializedProperty nameProp          = serializedObject.FindProperty("name");
        SerializedProperty iconProp          = serializedObject.FindProperty("icon");
        SerializedProperty rarityProp        = serializedObject.FindProperty("rarity");
        SerializedProperty descriptionProp   = serializedObject.FindProperty("description");

        SerializedProperty hasRuntimeValueProp = serializedObject.FindProperty("hasRuntimeValue");
        SerializedProperty runtimeFormatProp   = serializedObject.FindProperty("runtimeStatsFormat");

        // Draw top fields manually (same order as Unity normally does)
        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(iconProp);
        EditorGUILayout.PropertyField(rarityProp);
        EditorGUILayout.PropertyField(descriptionProp); // ⬅ Description box

        // ⬇ Place your runtime fields RIGHT BELOW description ⬇
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Runtime Value", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(hasRuntimeValueProp);

        if (hasRuntimeValueProp.boolValue)
        {
            if (string.IsNullOrEmpty(runtimeFormatProp.stringValue))
                runtimeFormatProp.stringValue = "+x";
            EditorGUILayout.PropertyField(runtimeFormatProp);
        }
            

        EditorGUILayout.Space(6);

        // Draw everything else except the ones we manually handled
        DrawPropertiesExcluding(serializedObject,
            "m_Script",
            "name",
            "icon",
            "rarity",
            "description",
            "hasRuntimeValue",
            "runtimeStatsFormat"
        );

        serializedObject.ApplyModifiedProperties();
    }
}