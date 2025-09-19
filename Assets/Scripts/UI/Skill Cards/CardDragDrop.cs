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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!cardUI.Selected) return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        raycastBlocker.GetComponent<Image>().raycastTarget = false;
        if (!TryDropOnTower())
        {
            rect.anchoredPosition = originalPosition;
            return;
        }
        OnDropCard?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!cardUI.Selected) return;
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    private bool TryDropOnTower()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null && hit.collider.GetComponent<IUsesCards>() != null)
        {
            return hit.collider.GetComponent<IUsesCards>().TryAddCard(cardUI.SkillData);
        }
        return false;
    }
    
}
