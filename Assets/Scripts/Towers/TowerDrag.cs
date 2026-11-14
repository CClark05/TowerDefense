using System;
using System.Collections;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDrag : MonoBehaviour
{
    private const float DragThreshold = 5f;
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
    public static event Action OnMovingStatic;
    public static event Action OnDoneMovingStatic;
    public event Action OnSelect;
    public event Action OnDeselect;
    private SelectionTile selectionTile;
    private TowerDataHolder towerDataHolder;
    private bool isInvalidSpot;
    private bool isSelected;
    public bool IsSelected => isSelected;
    private bool isPressed;
    private Vector2 mouseDownScreenPos;

    private void Awake()
    {
        cam = Camera.main;
        gridManager = GridManager.Instance;
        towerDataHolder = GetComponent<TowerDataHolder>();
        towerHoverable = GetComponent<TowerHoverable>();
    }

    private void Start()
    {
        PlayerCursor.Instance.OnClick += HandleCursorClick;
    }

    private void HandleCursorClick(IHoverable hovered)
    {
        if (!isSelected || hovered == towerHoverable) return;
        isSelected = false;
        OnDeselect?.Invoke();
    }

    private void Select()
    {
        if (isSelected) return;
        isSelected = true;
        OnSelect?.Invoke();
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        GameManager.Instance.SetCursor(GameManager.Cursors.ClosedHand);
        isPressed = true;
        mouseDownScreenPos = Input.mousePosition;
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        offset = (Vector2)transform.position - mousePosition;
    }

    void OnMouseDrag()
    {
        if (!isPressed) return;

        if (!isMoving)
        {
            if (Vector2.Distance(Input.mousePosition, mouseDownScreenPos) < DragThreshold) return;
            StartMoving();
        }

        Vector2 offsetPosition = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
        gridManager.Grid.GetXY(offsetPosition, out int x, out int y);
        Vector2 worldGridPosition = gridManager.Grid.GetWorldPosition(x, y) + Vector2.one * (gridManager.CellSize / 2f);
        transform.position = worldGridPosition;
        isInvalidSpot = gridManager.Grid.GetValue(x, y).id is not 0 || EventSystem.current.IsPointerOverGameObject();
        selectionTile.SetGreen(!isInvalidSpot);
        selectionTile.transform.position = worldGridPosition;
    }

    private void StartMoving()
    {
        isMoving = true;
        IsAnyMoving = true;
        originalPosition = transform.position;
        gridManager.Grid.GetXY(originalPosition, out int oldX, out int oldY);
        gridManager.SetEmpty(oldX, oldY);
        OnMoving?.Invoke();
        OnMovingStatic?.Invoke();
        selectionTile = Instantiate(BuildingManager.Instance.SelectionTilePrefab, originalPosition, Quaternion.identity).GetComponent<SelectionTile>();
    }

    private void OnMouseUp()
    {
        if (!isPressed) return;
        Physics2D.SyncTransforms();
        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        bool overTower = GetComponent<Collider2D>().OverlapPoint(mouseWorld);
        GameManager.Instance.SetCursor(overTower ? GameManager.Cursors.OpenHand : GameManager.Cursors.Default);
        isPressed = false;

        if (!isMoving)
        {
            Select();
            return;
        }

        IsAnyMoving = false;
        Vector2 position = isInvalidSpot ? originalPosition : transform.position;
        Destroy(selectionTile.gameObject);
        gridManager.Grid.GetXY(position, out int x, out int y);
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
            OnDoneMoving?.Invoke(true);
        }
        OnDoneMovingStatic?.Invoke();
        isMoving = false;
    }

    private IEnumerator UpdateTowers()
    {
        yield return null;
        foreach (var tower in TowerDataHolder.ActiveTowerList)
        {
            CallModifier.Call<IOnNewTowerAdded>(tower.SkillContext, (mod, instance) => mod.RefreshAffectedTowers());
        }
    }
    
    private void OnDestroy()
    {
        if (PlayerCursor.Instance != null)
            PlayerCursor.Instance.OnClick -= HandleCursorClick;
    }
}