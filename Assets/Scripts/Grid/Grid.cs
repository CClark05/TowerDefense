using System;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;

public class Grid<T>
{
    private int width, height;
    private float cellSize;
    private T[,] gridArray;
    private Vector2 originPosition;
    private TextMeshPro[,] debugTextArray;
    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;
    public Grid(int width, int height, int cellSize, Vector2 originPosition, Func<Grid<T>, int, int, T> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new T[width, height];
        debugTextArray = new TextMeshPro[width, this.height];

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = createGridObject(this, i, j);
                //debugTextArray[i, j] = HelperMethods.CreateWorldText(gridArray[i, j].ToString(), GetWorldPosition(i, j) + (new Vector2(cellSize, cellSize) / 2), sortingOrder:99);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 9999);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 9999);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 9999);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 9999);
    }

    public Vector2 GetWorldPosition(int x, int y) => new Vector2(x, y) * cellSize + originPosition;

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            //Debug.Log(gridArray[x, y]);
            //debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void GetXY(Vector2 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(Vector2 worldPosition, T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        return default;
    }
    
}