using TMPro;
using UnityEngine;

public static class HelperMethods
{
    
    public static TextMeshPro CreateWorldText(string text, Vector3 position, Transform parent = null, int fontSize = 4, Color? color = null, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
    {
        GameObject textObj = new GameObject("WorldText", typeof(TextMeshPro));
        if (parent != null) textObj.transform.SetParent(parent);
        
        textObj.transform.position = position;

        TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color ?? Color.white;
        textMesh.alignment = alignment;
        textMesh.enableWordWrapping = false;
        textMesh.isOverlay = false;

        return textMesh;
    }
    public static TextMeshPro CreateWorldText(
        string text,
        Vector3 position,
        Transform parent = null,
        int fontSize = 4,
        Color? color = null,
        TextAlignmentOptions alignment = TextAlignmentOptions.Center,
        int sortingOrder = 0)
    {
        GameObject textObj = new GameObject("WorldText", typeof(TextMeshPro));
        if (parent != null) textObj.transform.SetParent(parent);

        textObj.transform.position = position;

        TextMeshPro textMesh = textObj.GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color ?? Color.white;
        textMesh.alignment = alignment;
        textMesh.enableWordWrapping = false;
        textMesh.isOverlay = false;

        // Set sorting order
        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingOrder = sortingOrder;
        }

        return textMesh;
    }
    
    public static float ScaleForRadius(SpriteRenderer sr, float targetRadius)
    {
        float baseRadius = sr.sprite.bounds.extents.x; 
        return targetRadius / baseRadius;
    }
}
