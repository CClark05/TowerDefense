using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyWaveData))]
public class EnemyWaveDataDrawer : PropertyDrawer
{
    const float Spacing = 2f;
    const float FoldoutWidth = 14f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // Foldout toggle (no label)
        var foldRect = new Rect(line.x, line.y, FoldoutWidth, line.height);
        property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, GUIContent.none);

        // EnemyData slot stays visible even when collapsed
        var dataRect = new Rect(line.x + FoldoutWidth + 4f, line.y, line.width - (FoldoutWidth + 4f), line.height);
        EditorGUI.PropertyField(dataRect, property.FindPropertyRelative(nameof(EnemyWaveData.data)), GUIContent.none);

        // Draw extra fields only when expanded
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float y = line.y + line.height + Spacing;

            var countProp = property.FindPropertyRelative(nameof(EnemyWaveData.count));
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, line.height), countProp);
            y += line.height + Spacing;

            var delayProp = property.FindPropertyRelative(nameof(EnemyWaveData.delay));
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, line.height), delayProp);

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUIUtility.singleLineHeight; // header
        if (property.isExpanded)
            h += (EditorGUIUtility.singleLineHeight + Spacing) * 2; // count + delay
        return h;
    }
}