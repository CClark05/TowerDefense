using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class CardIconDragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rect;
    private RectTransform chestRect;
    private Canvas worldCanvas;
    private Canvas screenCanvas;
    private Camera cam;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private CardIconUI iconUI;
    public event Action OnRemoveCard;
    public event Action<bool> OnIsOverReceiverUpdated;
    public event Action<IUsesCards> OnOverTarget;
    public event Action OnLeftTarget;

    private bool isOverReceiver;
    public bool IsOverReceiver
    {
        get => isOverReceiver;
        set
        {
            if (value == isOverReceiver) return;
            isOverReceiver = value;
            OnIsOverReceiverUpdated?.Invoke(value);
        }
    }

    private IUsesCards cardReceiver;
    private IUsesCards lastHoverReceiver;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        cam = Camera.main;
        worldCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        screenCanvas = MainCanvas.Instance.GetComponent<Canvas>();
        iconUI = GetComponent<CardIconUI>();
    }

    private void Start()
    {
        chestRect = InventoryChestUI.Instance.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (EnemyManager.Instance.WaveState != EnemyManager.WaveStates.Idle) return;
        IsOverReceiver = false;
        lastHoverReceiver = null;
        originalPosition = rect.position;
        GameManager.Instance.SetCursor(GameManager.Cursors.ClosedHand);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (EnemyManager.Instance.WaveState != EnemyManager.WaveStates.Idle) return;
        cardReceiver = null;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                worldCanvas.transform as RectTransform, eventData.position, worldCanvas.worldCamera, out var world))
        {
            rect.position = world;
        }

        bool IsOverChest = RectTransformUtility.RectangleContainsScreenPoint(
            chestRect,
            eventData.position,
            screenCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : screenCanvas.worldCamera
        );
        if (IsOverChest) cardReceiver = InventoryUI.Instance.GetComponent<IUsesCards>();

        Vector2 pointerWorld = cam.ScreenToWorldPoint(eventData.position);
        var hit = Physics2D.Raycast(pointerWorld, Vector2.zero);
        if (hit.collider != null && hit.collider.GetComponent<IUsesCards>() != null)
            cardReceiver = hit.collider.GetComponent<IUsesCards>();

        if (cardReceiver != null && cardReceiver != lastHoverReceiver)
        {
            OnOverTarget?.Invoke(cardReceiver);
            lastHoverReceiver = cardReceiver;
        }
        else if (cardReceiver == null)
        {
            lastHoverReceiver = null;
            OnLeftTarget?.Invoke();
        }
        
        IsOverReceiver = cardReceiver != null && cardReceiver.CanAddCard(iconUI.SkillInstance);
        GameManager.Instance.SetCursor(IsOverReceiver ? GameManager.Cursors.OpenHand : GameManager.Cursors.ClosedHand);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (EnemyManager.Instance.WaveState != EnemyManager.WaveStates.Idle) return;
        GameManager.Instance.SetCursor(GameManager.Cursors.Default);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        lastHoverReceiver = null;

        if (isOverReceiver)
        {
            if (cardReceiver.CanAddCard(iconUI.SkillInstance))
            {
                iconUI.usesCards.RemoveCard(iconUI.SkillInstance);
                OnRemoveCard?.Invoke();
                cardReceiver.AddCard(iconUI.SkillInstance);
                Destroy(gameObject);
                return;
            }
            rect.position = originalPosition;
            return;
        }
        rect.position = originalPosition;
        IsOverReceiver = false;
    }
}
