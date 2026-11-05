using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDrag : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private GridManager gridManager;
    private Vector2 originalPosition; 
    private TowerHoverable towerHoverable;
    private bool isMoving;
    public bool IsMoving => isMoving;
    public static bool IsAnyMoving;
    public event Action OnMoving;
    public event Action<bool> OnDoneMoving; 
    private SelectionTile selectionTile;
    private TowerDataHolder towerDataHolder;
    private bool isInvalidSpot;
    private void Awake()
    {
        cam = Camera.main;
        gridManager = GridManager.Instance;
        towerDataHolder = GetComponent<TowerDataHolder>();
    }



    void OnMouseDown()
    {
        GameManager.Instance.SetCursor(GameManager.Cursors.ClosedHand);
        originalPosition = transform.position;
        gridManager.Grid.GetXY(originalPosition, out int oldX, out int oldY);
        gridManager.SetEmpty(oldX, oldY);
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = (Vector2)transform.position - mousePosition;
        OnMoving?.Invoke();
        selectionTile = Instantiate(BuildingManager.Instance.SelectionTilePrefab, originalPosition, Quaternion.identity).GetComponent<SelectionTile>();
    }

    private void OnMouseUp()
    {
        IsAnyMoving = false;
        if (isMoving)
        {
            Vector2 position = isInvalidSpot ? originalPosition : transform.position;
            Destroy(selectionTile.gameObject);
            GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
            gridManager.Grid.GetXY(position,out int x, out int y);
            gridManager.Grid.SetValue(x, y, towerDataHolder.Data);
            if (isInvalidSpot)
            {
                transform.position = originalPosition;
                OnDoneMoving?.Invoke(false);
            }
            else
            {
                //TODO MAKE SURE THIS WORKS
                StartCoroutine(UpdateTowers());
                IEnumerator UpdateTowers()
                {
                    yield return null;
                    foreach (var tower in TowerDataHolder.ActiveTowerList)
                    {
                        if(tower == towerDataHolder) continue;
                        CallModifier.Call<IOnNewTowerAdded>(tower.SkillContext, (mod, instance) => mod.OnNewTowerAdded(towerDataHolder));
                    }
                }
                OnDoneMoving?.Invoke(true);
            }
            isMoving = false;
        }
    }

    void OnMouseDrag()
    {
        isMoving = true;
        IsAnyMoving = true;
        Vector2 offsetPosition = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
        gridManager.Grid.GetXY(offsetPosition, out int x, out int y);
        Vector2 worldGridPosition = gridManager.Grid.GetWorldPosition(x, y) + Vector2.one * (gridManager.CellSize / 2f);
        transform.position = worldGridPosition;
        isInvalidSpot = gridManager.Grid.GetValue(x, y).id is not 0 || EventSystem.current.IsPointerOverGameObject();
        selectionTile.SetGreen(!isInvalidSpot);
        selectionTile.transform.position = worldGridPosition;
        
        /**
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(selectionTile.gameObject);
            transform.position = originalPosition;
            isMoving = false;
            OnDoneMoving?.Invoke(false);
        }
        */
    }
    

}