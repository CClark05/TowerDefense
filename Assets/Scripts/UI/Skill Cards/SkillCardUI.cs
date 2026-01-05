using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardUI : MonoBehaviour, ICardUI
{
    public SkillInstance SkillInstance { get; private set; }
    [SerializeField] private Button_Hover button;
    //[SerializeField] private Button_Base sellButton;
    public event Action<bool> OnClickedCard;
    public bool Selected { get; private set; }
    private static SkillCardUI selectedCard;
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
        //sellButton.gameObject.SetActive(false);
        GetComponent<Canvas>().sortingLayerName = "World UI";
        button.OnClick.AddListener(() =>
        {
            if (disableOnClick)
            {
                OnDisableButton?.Invoke();
                Destroy(gameObject);
                return;
            }
            Selected = !Selected;
            OnClickedCard?.Invoke(Selected);
           // sellButton.gameObject.SetActive(Selected);
            if (Selected)
            {
                if (selectedCard != null && selectedCard != this)
                {
                    selectedCard.Deselect();
                    OnHoverCard?.Invoke();
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
            OnHoverCard?.Invoke();
        };
        
        
        button.OnLeaveHover += () =>
        {
            if (Selected) return;
            ToggleButtonSize(true); 
            OnLeaveHoverCard?.Invoke();
        };
        /**
        sellButton.OnClick.AddListener(() =>
        {
            Selected = false;
            selectedCard = null;
            OnSellCardStatic?.Invoke(GetComponent<SetCardData>().SellPrice);
            OnSellCard?.Invoke();
            SkillInstance.Dispose();
        });
        */
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
        OnClickedCard?.Invoke(false);
        //sellButton.gameObject.SetActive(false);
        //GetComponent<Canvas>().sortingOrder = originalSortingOrder;
    }

    private void ToggleButtonSize(bool fullSize) => button.GetComponent<RectTransform>().localScale = fullSize ? originalScale : new Vector3(originalScale.x, originalScale.y / 2f, originalScale.z);
    
    //public void ToggleSellButton(bool show) => sellButton.gameObject.SetActive(show);
    public void ToggleButtonOnClick(bool enable) => disableOnClick = !enable;
}