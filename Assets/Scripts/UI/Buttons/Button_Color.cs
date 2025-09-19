using UnityEngine;

public class Button_Color : Button_Base
{
    [SerializeField] private Color newColor;

    public override void OnMouseEnter()
    {
        image.color = newColor;
    }

    public override void OnMouseLeave()
    {
        image.color = originalColor;
    }
}