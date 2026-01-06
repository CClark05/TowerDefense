using System;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour, ICardUI
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button_Hover button;
    [SerializeField] private GameObject UI;
    public event Action OnHoverCard;
    public event Action OnLeaveHoverCard;
    public event Action OnClickCard;
    public event Action OnRemoveCard;
    public GameObject GameObject => gameObject;
    private int count;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
        button.OnHover += () => OnHoverCard?.Invoke();
        button.OnLeaveHover += () => OnLeaveHoverCard?.Invoke();
        button.OnClick.AddListener(() =>
        {
            OnClickCard?.Invoke();
            transform.localScale = originalScale;
        });
    }

    public void ToggleVisibility(bool visible)
    {
        UI.SetActive(visible);
    }

    public void ToggleButton(bool enabled)
    {
        button.enabled = enabled;
    }
    public void Init(int count)
    {
        this.count = count;
        countText.text = count > 1 ? $"x{count}" : "";
    }

    public void AddCount(int amount, float delay)
    {
        count += amount;
        FunctionTimer.Create(() =>
        { 
            countText.text = count > 1 ? $"x{count}" : "";
            if (count <= 0)
            {
                OnRemoveCard?.Invoke();
                Destroy(gameObject);
            }
        }, delay);

    }

}
