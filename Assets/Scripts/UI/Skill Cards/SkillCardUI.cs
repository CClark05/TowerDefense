using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class SkillCardUI : MonoBehaviour
{
    public SkillInstance SkillInstance { get; private set; }
    [SerializeField] private Button_Hover button;
    [SerializeField] private Button_Base sellButton;
    public event Action<bool> OnClickCard;
    public bool Selected { get; private set; }
    private static SkillCardUI selectedCard;
    public static event Action<int> OnSellCardStatic;
    public event Action OnSellCard;
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;

    private static readonly List<SkillCardUI> activeCards = new();
    private int originalSortingOrder;
    private Vector3 originalScale;
    private void Start()
    {
        if (!activeCards.Contains(this)) activeCards.Add(this);

        var canvas = GetComponent<Canvas>();
        originalSortingOrder = canvas.sortingOrder;
        originalScale = button.GetComponent<RectTransform>().localScale;
        SkillInstance = GetComponent<SetCardData>().SkillInstance;
        sellButton.gameObject.SetActive(false);

        button.OnClick.AddListener(() =>
        {
            if (disableOnClick)
            {
                OnDisableButton?.Invoke();
                Destroy(gameObject);
                return;
            }
            Selected = !Selected;
            OnClickCard?.Invoke(Selected);
            sellButton.gameObject.SetActive(Selected);
            if (Selected)
            {
                if(!IsBottomMostChild())
                    ToggleButtonSize(false);
                if (selectedCard != null && selectedCard != this)
                {
                    selectedCard.Deselect();
                    OnHoverCard?.Invoke();
                    canvas.sortingOrder = 99;
                }
                selectedCard = this;
                return;
            }

            if (selectedCard == this)
            {
                selectedCard = null;
                ToggleButtonSize(true);
            }
        });

        button.OnHover += () =>
        {
            if (selectedCard != null) return;
            originalSortingOrder = canvas.sortingOrder;
            canvas.sortingOrder = 99;
            if(!IsBottomMostChild())
                ToggleButtonSize(false);
            OnHoverCard?.Invoke();
        };
        
        
        button.OnLeaveHover += () =>
        {
            if (Selected) return;
            canvas.sortingOrder = originalSortingOrder;
            ToggleButtonSize(true); 
            OnLeaveHoverCard?.Invoke();
        };

        sellButton.OnClick.AddListener(() =>
        {
            Selected = false;
            selectedCard = null;
            OnSellCardStatic?.Invoke(GetComponent<SetCardData>().SellPrice);
            OnSellCard?.Invoke();
            SkillInstance.Dispose();
        });
    }

    private bool disableOnClick;
    public event Action OnDisableButton;
    public event Action<SkillCardUI> OnRemoveCard;
    
    public void RemoveCard(SkillCardUI card)
    {
        OnRemoveCard?.Invoke(card);
        activeCards.Remove(this);
    }

    private void Deselect()
    {
        Selected = false;
        ToggleButtonSize(true);
        OnLeaveHoverCard?.Invoke();
        OnClickCard?.Invoke(false);
        sellButton.gameObject.SetActive(false);
        GetComponent<Canvas>().sortingOrder = originalSortingOrder;
    }

    private void ToggleButtonSize(bool fullSize) => button.GetComponent<RectTransform>().localScale = fullSize ? originalScale : new Vector3(originalScale.x, originalScale.y / 2f, originalScale.z);

    private bool IsBottomMostChild()
    {
        var p = transform.parent;
        for (int i = p.childCount - 1; i >= 0; --i)
        {
            if (p.GetChild(i).GetComponent<SkillCardUI>() != null)
                return i == transform.GetSiblingIndex();
        }
        return false;
    }
    public void ToggleSellButton(bool show) => sellButton.gameObject.SetActive(show);
    public void ToggleButtonOnClick(bool enable) => disableOnClick = !enable;
}