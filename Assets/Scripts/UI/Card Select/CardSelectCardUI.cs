using System;
using UnityEngine;

public class CardSelectCardUI : MonoBehaviour, ICardUI
{
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    public event Action OnClickCard;
    public GameObject GameObject => gameObject;
    public void ToggleButton(bool enabled)
    {
    }

    [SerializeField] private Button_Hover button;

    private void Start()
    {
        button.OnHover += () => OnHoverCard?.Invoke();
        button.OnLeaveHover += () => OnLeaveHoverCard?.Invoke();
        button.OnClick.AddListener(() => OnClickCard?.Invoke());
    }
    
}
