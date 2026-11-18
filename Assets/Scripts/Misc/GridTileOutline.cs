using System;
using UnityEngine;

public class GridTileOutline : Singleton<GridTileOutline>
{
    private GridManager gridManager;
    private SpriteRenderer sr;
    private void Awake()
    {
        gridManager = GridManager.Instance;
        sr = GetComponent<SpriteRenderer>();
    }

    public void ShowOutline(Vector2 position)
    {
        sr.enabled = true;
        transform.position = position;
    }

    public void HideOutline()
    {
        sr.enabled = false;
    }
}
