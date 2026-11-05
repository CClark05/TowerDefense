using System;
using System.Collections;
using Mono.Cecil;
using UnityEditor.Build.Content;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LightTransport;

public class BuildingManager : Singleton<BuildingManager>
{
    [SerializeField] private BuildButtonUI buildButtonUI;
    [SerializeField] private GameObject selectionTilePrefab;
    public GameObject SelectionTilePrefab => selectionTilePrefab;
    public event Action OnEnterBuildMode;
    public event Action OnExitBuildMode;
    private (GameObject preview, SelectionTile selectionTile, TowerData data) currentBuild;
    private GridManager gridManager;
    private Camera cam;
    public event Action<TowerData> OnPlacedBuild;
    public bool IsInBuildMode => currentBuild.preview != null;
    public bool IsInMoveMode { get; private set; }
    private (TowerDataHolder data, Vector2Int gridPos, int cost) mostRecentBuild;
    public event Action<TowerDataHolder> OnAddNewTower;
    private void Start()
    {
        gridManager = GridManager.Instance;
        cam = Camera.main;
        buildButtonUI.OnBuildMode += EnterBuildMode;
        buildButtonUI.OnExitBuildMode += ExitBuildMode;
    }

    public void EnterBuildMode(TowerData data)
    {
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        currentBuild.preview = Instantiate(data.prefab, mousePosition, Quaternion.identity);
        currentBuild.preview.GetComponent<BoxCollider2D>().enabled = false;
        currentBuild.data = data;
        currentBuild.selectionTile = Instantiate(selectionTilePrefab, mousePosition, Quaternion.identity).GetComponent<SelectionTile>();
        OnEnterBuildMode?.Invoke();
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
            var towerData = newBuilding.GetComponent<TowerDataHolder>();
            OnAddNewTower?.Invoke(towerData);
            mostRecentBuild.data = newBuilding.GetComponent<TowerDataHolder>();
            mostRecentBuild.gridPos = new Vector2Int(x, y);
            mostRecentBuild.cost = currentBuild.data.cost;
            OnPlacedBuild?.Invoke(currentBuild.data);
            newBuilding.GetComponent<TowerAnimation>().Init();
            //currentBuild.data.Cost += currentBuild.data.costIncreasePerPurchase;
            ExitBuildMode();
            StartCoroutine(UpdateTowers());
            IEnumerator UpdateTowers()
            {
                yield return null;
                foreach (var tower in TowerDataHolder.ActiveTowerList)
                {
                    if(tower == newBuilding.GetComponent<TowerDataHolder>()) continue;
                    CallModifier.Call<IOnNewTowerAdded>(tower.SkillContext, (mod, instance) => mod.OnNewTowerAdded(newBuilding.GetComponent<TowerDataHolder>()));
                }
            }
            
        }
    }
    

    public IEnumerator MoveBuild(GameObject build)
    {
        IsInMoveMode = true;
        Vector2 oldPosition = build.transform.position;
        var selectionTile = Instantiate(selectionTilePrefab, cam.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity).GetComponent<SelectionTile>();
        gridManager.Grid.GetXY(oldPosition, out int oldX, out int oldY);
        gridManager.SetEmpty(oldX, oldY);
        while (true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(selectionTile.gameObject);
                build.transform.position = oldPosition;
                IsInMoveMode = false;
                yield break;
            }
            Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            gridManager.Grid.GetXY(mousePosition, out int x, out int y);
            if (x < 0 || y < 0 || x >= gridManager.Grid.Width || y >= gridManager.Grid.Height) continue;
            Vector2 worldGridPosition = gridManager.Grid.GetWorldPosition(x, y) + Vector2.one * (gridManager.CellSize / 2f);
            build.transform.position = worldGridPosition;
            bool isInvalid = gridManager.Grid.GetValue(x, y).id is not 0 || EventSystem.current.IsPointerOverGameObject();
            selectionTile.SetGreen(!isInvalid);
            selectionTile.transform.position = worldGridPosition;
            if(isInvalid) continue;
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                IsInMoveMode = false;
                gridManager.Grid.SetValue(x, y, currentBuild.data);
                Destroy(selectionTile.gameObject);
                StartCoroutine(UpdateTowers());
                IEnumerator UpdateTowers()
                {
                    yield return null;
                    foreach (var tower in TowerDataHolder.ActiveTowerList)
                    {
                        if(tower == build.GetComponent<TowerDataHolder>()) continue;
                        CallModifier.Call<IOnNewTowerAdded>(tower.SkillContext, (mod, instance) => mod.OnNewTowerAdded(build.GetComponent<TowerDataHolder>()));
                    }
                }

                yield break;
            }
        }
    }
    
    public void ExitBuildMode()
    {
        if (currentBuild.preview != null)
        {
            Destroy(currentBuild.preview);
            Destroy(currentBuild.selectionTile.gameObject);
            currentBuild = (null, null, null);
        }

        OnExitBuildMode?.Invoke();
    }

    public void ToggleBuildPreview(bool enabled)
    {
        if (currentBuild.preview == null) return;
        currentBuild.preview.GetComponent<TowerVisual>().ToggleTowerVisibility(enabled);
        currentBuild.selectionTile.gameObject.SetActive(enabled);
    }
}