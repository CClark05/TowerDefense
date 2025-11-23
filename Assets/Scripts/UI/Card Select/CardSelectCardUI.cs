using System;
using UnityEngine;

public class CardSelectCardUI : MonoBehaviour, ICardUI
{
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    [SerializeField] private Button_Hover button;

    private void Start()
    {
        button.OnHover += () => OnHoverCard?.Invoke();
        button.OnLeaveHover += () => OnLeaveHoverCard?.Invoke();
    }
    
    
}
