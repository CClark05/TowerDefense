using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

public class BuildingManager : Singleton<BuildingManager>
{
    private (GameObject preview, BuildableObjectData data) currentBuilding;
    private Camera camera;
    private GridManager gridManager;
    private Color originalColor;
    private float opacity = 0.5f;
    private SpriteRenderer sr;
    public event Action OnEnterBuildMode;
    public event Action OnExitBuildMode;
    public bool IsBuildMode { get; private set; }
    public event Action<Dictionary<ResourceData, int>> OnPlacedBuilding;
    public Action OnCancelBuild;
    [SerializeField] private GameObject selectionTilePrefab;
    private GameObject selectionTile;
    private new void Awake()
    {
        base.Awake();
        camera = Camera.main;
    }

    private void Start()
    {
        gridManager = GridManager.Instance;
        BuildingUI.Instance.OnButtonPress += OnButtonPress;
        BuildingUI.Instance.OnEnterBuildMode += () =>
        {
            IsBuildMode = true;
        };
        BuildingUI.Instance.OnExitBuildMode += () =>
        {
            IsBuildMode = false;
        };
        
        BuildingUI.Instance.OnTrash += () =>
        {
            Destroy(currentBuilding.preview);
            Destroy(selectionTile);
        };
    }

    private void OnButtonPress(BuildableObjectData data)
    {
        currentBuilding.data = data;
        currentBuilding.preview = Instantiate(data.prefab);
        currentBuilding.preview.GetComponent<BoxCollider2D>().enabled = false;
        selectionTile = Instantiate(selectionTilePrefab);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsBuildMode = !IsBuildMode;
            if (IsBuildMode)
            {
                OnEnterBuildMode?.Invoke();
                return;
            }

            Destroy(currentBuilding.preview);
            Destroy(selectionTile);
            OnExitBuildMode?.Invoke();
        }

        if (currentBuilding.preview == null) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(currentBuilding.preview);
            Destroy(selectionTile);
            OnCancelBuild?.Invoke();
        }
            

        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        gridManager.Grid.GetXY(mousePosition, out int x, out int y);
        if (x < 0 || y < 0 || x >= gridManager.Grid.Width || y >= gridManager.Grid.Height) return;
        Vector2 worldGridPosition = gridManager.Grid.GetWorldPosition(x, y) + Vector2.one * (gridManager.CellSize / 2f);
        currentBuilding.preview.transform.position = worldGridPosition;
        var playerPosition = PlayerTurnManager.Instance.transform.position;
        gridManager.Grid.GetXY(playerPosition, out int playerX, out int playerY);

        bool isInvalid = gridManager.Grid.GetValue(x, y).id is not (0 or 6) ||
                         !HasEnoughResources(currentBuilding.data.CostDictionary) || EventSystem.current.IsPointerOverGameObject() || (playerX == x && playerY == y)
                         || Vector2.Distance(worldGridPosition, playerPosition) > PlayerCursor.Instance.Range || PlayerTurnManager.Instance.MovesRemaining <= 0;

        selectionTile.GetComponent<SelectionTile>().SetGreen(!isInvalid);
        selectionTile.transform.position = worldGridPosition;
        if (isInvalid) return;
        if (Input.GetMouseButtonDown(0))
        {
            gridManager.Grid.SetValue(x, y, currentBuilding.data);
            GameObject newBuilding = Instantiate(currentBuilding.data.prefab, worldGridPosition, Quaternion.identity);
            OnPlacedBuilding?.Invoke(currentBuilding.data.CostDictionary);
            Destroy(currentBuilding.preview);
            Destroy(selectionTile);
        }
    }

    private bool HasEnoughResources(Dictionary<ResourceData, int> dict)
    {
        foreach (var kvp in dict)
        {
            if (PlayerInventory.Instance.GetAmount(kvp.Key) < kvp.Value)
                return false;
        }
        return true;
    }
}