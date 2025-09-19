using System;
using UnityEngine;

public class SelectionTile : MonoBehaviour
{
    [SerializeField] private Sprite redSprite;
    private Sprite greenSprite;
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        greenSprite = sr.sprite;
    }

    public void SetGreen(bool green)
    {
        if (green)
        {
            sr.sprite = greenSprite;
            return;
        }

        sr.sprite = redSprite;
    }
}
