using System;
using UnityEngine;

public class InputCardUI : MonoBehaviour, ICardUI
{
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    public event Action OnClickCard;
    [SerializeField] private Button_Hover button;
    public GameObject GameObject => gameObject;
    public void ToggleButton(bool enabled)
    {
        button.enabled = enabled;
    }

    private void Start()
    {
        button.OnHover += () => OnHoverCard?.Invoke();
        button.OnLeaveHover += () => OnLeaveHoverCard?.Invoke();
        button.OnClick.AddListener(() =>
        {
            OnClickCard?.Invoke();
        });
    }
}
