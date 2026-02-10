using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    private SkillCardUI cardUI;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private UIRaycastBlocker raycastBlocker;
    private Transform dragParent;
    private Transform originalParent;
    public event Action OnDropCard;
    public event Action OnOverTarget;
    public Vector2? TargetPosition { get; private set; } = null;
    public bool IsDragging { get; private set; }
    public static bool IsDraggingAny { get; private set; }
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        cardUI = GetComponent<SkillCardUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        raycastBlocker = UIRaycastBlocker.Instance;
        dragParent = transform.parent.parent.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager.Instance.SetCursor(GameManager.Cursors.ClosedHand);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
        //originalPosition = rect.anchoredPosition + new Vector2(0,-50f);
        originalParent = rect.parent;
        rect.SetParent(dragParent, true);

        raycastBlocker.GetComponent<Image>().raycastTarget = true;
        IsDragging = true;
        IsDraggingAny = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        raycastBlocker.GetComponent<Image>().raycastTarget = false;
        IsDragging = false;
        IsDraggingAny = false;
        if (!TryDropOnTower())
        {
            rect.SetParent(originalParent, true);
            TargetPosition = GetComponent<InventoryCardAnimation>().originalPos;
        }
        else
        {
            TargetPosition = rect.anchoredPosition;
        }
    }

    private IUsesCards lastHit;

    public void OnDrag(PointerEventData eventData)
    {
        TargetPosition = rect.anchoredPosition + eventData.delta / canvas.scaleFactor;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        IUsesCards hovered = null;
        if (hit.collider != null)
            hovered = hit.collider.GetComponent<IUsesCards>();

        if (hovered != null && hovered != lastHit)
        {
            if (hovered.CanAddCard(cardUI.SkillInstance))
            {
                OnOverTarget?.Invoke();
                GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
                lastHit = hovered;
                return;
            }
            GameManager.Instance.SetCursor(GameManager.Cursors.Disabled);
        }

        if (hovered == null && (lastHit != null || GameManager.Instance.CurrentCursor == GameManager.Cursors.Disabled))
        {
            lastHit = null;
            GameManager.Instance.SetCursor(GameManager.Cursors.ClosedHand);
        }
    }

    private bool TryDropOnTower()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        GameManager.Instance.SetCursor(GameManager.Cursors.Default);
        if (hit.collider != null && hit.collider.GetComponent<IUsesCards>() != null)
        {
            if (hit.collider.GetComponent<IUsesCards>().CanAddCard(cardUI.SkillInstance))
            {
                OnDropCard?.Invoke();
                hit.collider.GetComponent<IUsesCards>().AddCard(cardUI.SkillInstance);
                return true;
            }
        }
        return false;
    }

    public void ClearTarget()
    {
        TargetPosition = null;
    }
}