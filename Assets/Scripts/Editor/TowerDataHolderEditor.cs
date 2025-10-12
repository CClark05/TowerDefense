// Editor/TowerDataHolderEditor.cs
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TowerDataHolder))]
public class TowerDataHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = (TowerDataHolder)target;
        if (!Application.isPlaying || t == null || t.SkillContext == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Active Skills (runtime)", EditorStyles.boldLabel);

        var list = t.SkillContext.ActiveSkills;
        for (int i = 0; i < list.Count; i++)
        {
            var inst = list[i];
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField($"#{i} {inst.GetType().Name}", EditorStyles.miniBoldLabel);
                EditorGUILayout.ObjectField("Data", inst.Data, typeof(SkillData), false);
                EditorGUILayout.IntField("Play Count (live)", inst.PlayCount);
                // Optional: show an instance id to correlate with logs
                EditorGUILayout.LabelField("Instance", inst.GetHashCode().ToString());
            }
        }

        // Force live refresh while playing
        if (Event.current.type == EventType.Repaint)
            Repaint();
    }
}