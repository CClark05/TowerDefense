using System;
using System.Linq;
using UnityEngine;

public class GridObjectButton : MonoBehaviour, IHoverable
{
    public SOEvent OnClickObject;
    public SpriteRenderer[] Sprites { get; private set; }
    private void Awake()
    {
        Sprites = GetComponentsInChildren<SpriteRenderer>().OrderBy(sr => sr.transform.GetSiblingIndex()).ToArray();
    }

    public void OnHover()
    {
        foreach (SpriteRenderer sr in Sprites)
            sr.color = new Color(1, 1, 1, 0.5f);
    }
    public void OnLeaveHover()
    {
        foreach (SpriteRenderer sr in Sprites)
            sr.color = new Color(1, 1, 1, 1);
    }
    public void OnClick()
    {
        OnClickObject.Raise(this);
    }
    
}
