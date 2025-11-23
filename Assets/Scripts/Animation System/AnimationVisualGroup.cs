using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class AnimationVisualGroup : MonoBehaviour
{
    [Serializable]
    private struct Entry { public int id; public AnimationVisualID comp; }

    [SerializeField] private int nextId = 0;
    [SerializeField] private List<Entry> entries = new();   // serialized = stable
    private readonly Dictionary<int, Transform> map = new();

    private void Awake()
    {
        foreach (var v in GetComponentsInChildren<AnimationVisualID>(true))
            if (v != null) map[v.ID] = v.transform;
    }

    public Transform TryGet(int id)
    {
        map.TryGetValue(id, out var t);
        return t;
    }

#if UNITY_EDITOR
    public void EnsureRegistered(AnimationVisualID v)
    {
        if (!v) return;

        // already registered?
        for (int i = 0; i < entries.Count; i++)
            if (entries[i].comp == v) return;

        // if this instance came in with a copied ID (due to duplication), IGNORE it
        var e = new Entry { id = nextId++, comp = v };
        entries.Add(e);

        // write back to the child
        var so = new SerializedObject(v);
        so.FindProperty("id").intValue = e.id;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(v);
    }
#endif
}