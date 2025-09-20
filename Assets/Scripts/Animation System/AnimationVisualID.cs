using UnityEngine;

public class AnimationVisualID : MonoBehaviour
{
    [SerializeField, HideInInspector] private int id = -1;
    public int ID => id;

#if UNITY_EDITOR
    internal void __Editor_SetId(int newId) => id = newId;
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        GetComponentInParent<AnimationVisualGroup>()?.EnsureRegistered(this);
    }
#endif
}
