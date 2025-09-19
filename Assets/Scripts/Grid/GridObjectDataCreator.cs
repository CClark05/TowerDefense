#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public class GridObjectDataCreator
{
    [MenuItem("Assets/Create/Grid/Grid Object With Auto ID")]
    public static void CreateGridObjectData()
    {
        // Find all existing GridObjectData assets
        string[] guids = AssetDatabase.FindAssets("t:GridObjectData");
        int maxId = -1;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GridObjectData data = AssetDatabase.LoadAssetAtPath<GridObjectData>(path);
            if (data != null)
                maxId = Mathf.Max(maxId, data.id);
        }

        // Create new asset
        GridObjectData newData = ScriptableObject.CreateInstance<GridObjectData>();
        newData.id = maxId + 1;
        newData.objectName = $"Object_{newData.id}";

        string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects/GridObjectData/NewGridObjectData.asset");
        AssetDatabase.CreateAsset(newData, assetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newData;

        Debug.Log($"Created new GridObjectData with ID {newData.id}");
    }
}
#endif