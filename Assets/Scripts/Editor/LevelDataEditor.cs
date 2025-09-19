using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private GridObjectData selectedTile;
    private bool eraserMode = false;

    public override void OnInspectorGUI()
    {
        LevelData level = (LevelData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Tile Painter", EditorStyles.boldLabel);

        // Grid initializer
        if (GUILayout.Button("Initialize Grid (Scene View)"))
        {
            level.InitializeGrid();
            EditorUtility.SetDirty(level);
            Debug.Log($"[LevelData] Initialized grid using Scene View: {level.width}x{level.height}");
        }

        EditorGUILayout.Space();

        // Eraser toggle
        eraserMode = EditorGUILayout.Toggle("Eraser Mode", eraserMode);

        // Only allow selecting tile when not erasing
        if (!eraserMode)
        {
            selectedTile = (GridObjectData)EditorGUILayout.ObjectField("Selected Tile", selectedTile, typeof(GridObjectData), false);
        }

        if (level.tiles == null || level.tiles.Length != level.width * level.height)
        {
            EditorGUILayout.HelpBox("Grid is not initialized. Click 'Initialize Grid (Scene View)' above.", MessageType.Warning);
            return;
        }

        // Grid buttons
        for (int y = level.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < level.width; x++)
            {
                GridObjectData tile = level.GetTile(x, y);
                string label = tile != null ? tile.id.ToString() : ".";

                if (GUILayout.Button(label, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    if (eraserMode)
                    {
                        level.SetTile(x, y, null);
                    }
                    else
                    {
                        level.SetTile(x, y, selectedTile);
                    }

                    EditorUtility.SetDirty(level);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}