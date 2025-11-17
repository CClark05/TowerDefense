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
    public event Action OnDropCard;
    public Vector2? TargetPosition { get; private set; } = null;
    public bool IsDragging { get; private set; }
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!cardUI.Selected) return;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;
        originalPosition = rect.anchoredPosition;
        raycastBlocker.GetComponent<Image>().raycastTarget = true;
        IsDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!cardUI.Selected) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        raycastBlocker.GetComponent<Image>().raycastTarget = false;
        IsDragging = false;
        if (!TryDropOnTower())
        {
            //rect.anchoredPosition = originalPosition;
            TargetPosition = originalPosition;
            return;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!cardUI.Selected) return;
        TargetPosition = rect.anchoredPosition + eventData.delta / canvas.scaleFactor;
        //rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    private bool TryDropOnTower()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null && hit.collider.GetComponent<IUsesCards>() != null)
        {
            if (hit.collider.GetComponent<IUsesCards>().CanAddCard(cardUI.SkillInstance.Data))
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
