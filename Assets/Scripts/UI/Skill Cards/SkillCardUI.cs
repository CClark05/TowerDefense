using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardUI : MonoBehaviour, ICardUI
{
    public SkillInstance SkillInstance { get; private set; }
    [SerializeField] private Button_Hover button;
    //[SerializeField] private Button_Base sellButton;
    private CardDragDrop dragDrop;
    public static event Action<int> OnSellCardStatic;
    //public event Action OnSellCard;
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    public event Action OnClickCard;
    public GameObject GameObject => gameObject;
    public void ToggleButton(bool enabled)
    {
        button.enabled = enabled;
    }

    private static readonly List<SkillCardUI> activeCards = new();
    private Vector3 originalScale;
    private void Start()
    {
        if (!activeCards.Contains(this)) activeCards.Add(this);
        originalScale = button.GetComponent<RectTransform>().localScale;
        SkillInstance = GetComponent<SetCardData>().SkillInstance;
        dragDrop = GetComponent<CardDragDrop>();
        GetComponent<Canvas>().sortingLayerName = "World UI";
        
        button.OnHover += () =>
        {
            if (CardDragDrop.IsDraggingAny) return;
            OnHoverCard?.Invoke();
        };
        
        button.OnLeaveHover += () =>
        {
            if (CardDragDrop.IsDraggingAny) return;
            ToggleButtonSize(true); 
            OnLeaveHoverCard?.Invoke();
        };
    }
    
    public event Action<SkillCardUI> OnRemoveCard;
    
    public void RemoveCard(SkillCardUI card)
    {
        OnRemoveCard?.Invoke(card);
        activeCards.Remove(this);
    }
    

    private void ToggleButtonSize(bool fullSize) => button.GetComponent<RectTransform>().localScale = fullSize ? originalScale : new Vector3(originalScale.x, originalScale.y / 2f, originalScale.z);

}