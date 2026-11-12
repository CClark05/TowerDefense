using System;
using DG.Tweening;
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
        transform.DOScale(new Vector2(0.8f, 0.8f), 0.75f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
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
