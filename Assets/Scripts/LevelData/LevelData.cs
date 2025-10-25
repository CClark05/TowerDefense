using UnityEngine;

[CreateAssetMenu(fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Grid Data")]
    public int width;
    public int height;
    public int cellSize = 2;
    public float orthoSize;
    public Vector2 origin;
    public GridObjectData[] tiles;
    public GridObjectData emptyTile;
    [Header("Player Data")] 
    public int playerLives;
    public void InitializeGrid()
    {
        float aspect = 16 / 9f;
        origin = new Vector2(
            -orthoSize * aspect,
            -orthoSize);

        tiles = new GridObjectData[width * height];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = emptyTile;
        }
    }

    public GridObjectData GetTile(int x, int y) => tiles[y * width + x];
    public void SetTile(int x, int y, GridObjectData data) => tiles[y * width + x] = data;
}