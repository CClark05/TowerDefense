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
    public event Action<SkillData> OnRemoveCard;
    public event Action<bool> OnIsOverReceiverUpdated;
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
        IsOverReceiver = false;
        originalPosition = rect.position;
        if (!iconUI.Selected) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!iconUI.Selected) return;
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
        {
            cardReceiver = hit.collider.GetComponent<IUsesCards>();
        }

        IsOverReceiver = cardReceiver != null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!iconUI.Selected) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        if (isOverReceiver)
        {
            if (cardReceiver.TryAddCard(iconUI.SkillData))
            {
                iconUI.usesCards.RemoveCard(iconUI.SkillData);
                OnRemoveCard?.Invoke(iconUI.SkillData);
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