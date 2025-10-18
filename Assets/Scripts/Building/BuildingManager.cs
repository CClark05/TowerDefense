using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : Singleton<BuildingManager>
{
    [SerializeField] private BuildButtonUI buildButtonUI;
    [SerializeField] private GameObject selectionTilePrefab;
    public event Action OnEnterBuildMode;
    public event Action OnExitBuildMode;
    private (GameObject preview, SelectionTile selectionTile, TowerData data) currentBuild;
    private GridManager gridManager;
    private Camera cam;
    public event Action<TowerData> OnPlacedBuild;
    public bool IsInBuildMode => currentBuild.preview != null;
    private void Start()
    {
        gridManager = GridManager.Instance;
        cam = Camera.main;
        buildButtonUI.OnBuildMode += data =>
        {
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            currentBuild.preview = Instantiate(data.prefab, mousePosition, Quaternion.identity);
            currentBuild.preview.GetComponent<BoxCollider2D>().enabled = false;
            currentBuild.data = data;
            currentBuild.selectionTile = Instantiate(selectionTilePrefab, mousePosition, Quaternion.identity).GetComponent<SelectionTile>();
            OnEnterBuildMode?.Invoke();
        };
        buildButtonUI.OnExitBuildMode += ExitBuildMode;
    }

    private void Update()
    {
        if (currentBuild.preview == null) return;
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        gridManager.Grid.GetXY(mousePosition, out int x, out int y);
        if (x < 0 || y < 0 || x >= gridManager.Grid.Width || y >= gridManager.Grid.Height) return;
        Vector2 worldGridPosition = gridManager.Grid.GetWorldPosition(x, y) + Vector2.one * (gridManager.CellSize / 2f);
        currentBuild.preview.transform.position = worldGridPosition;
        currentBuild.selectionTile.transform.position = worldGridPosition;
        bool isInvalid = gridManager.Grid.GetValue(x, y).id is not 0 || EventSystem.current.IsPointerOverGameObject();
        currentBuild.selectionTile.SetGreen(!isInvalid);
        if (isInvalid) return;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            gridManager.Grid.SetValue(x, y, currentBuild.data);
            GameObject newBuilding = Instantiate(currentBuild.data.prefab, worldGridPosition, Quaternion.identity);
            OnPlacedBuild?.Invoke(currentBuild.data);
            ExitBuildMode();
        }
    }
    private void ExitBuildMode()
    {
        Destroy(currentBuild.preview);
        Destroy(currentBuild.selectionTile.gameObject);
        currentBuild = (null, null, null);
        OnExitBuildMode?.Invoke();
    }
}