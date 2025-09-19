using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryChestUI : Singleton<InventoryChestUI>
{
    public enum States
    {
        Open,
        Closed,
        Peeking
    }

    public States state = States.Open;
    private Sprite openSprite;
    [SerializeField] private Sprite peekingSprite;
    [SerializeField] private Sprite closedSprite;
    private Image image;
    public bool RemainOpen { get; private set; } = true;
    private new void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
        openSprite = image.sprite;
    }

    public void ToggleRemainOpen(bool open) => RemainOpen = open;

    public void SetState(States state)
    {
        this.state = state;
        switch (state)
        {
            case States.Open:
                image.sprite = openSprite;
                break;
            case States.Closed:
                image.sprite = closedSprite;
                break;
            case States.Peeking:
                image.sprite = peekingSprite;
                break;
        }
    }
}
