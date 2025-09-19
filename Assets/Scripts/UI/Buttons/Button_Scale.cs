using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Button_Scale : Button_Base
{
    [SerializeField] protected float scaleFactor;
    [SerializeField] protected float animationDuration;
    [SerializeField] protected Sprite pressedButtonSprite;
    //SerializeField] private AudioClip clickSound;
    //[SerializeField] private AudioClip hoverSound;
    private Sprite originalSprite;
    private bool pauseScaleAnimation;
    private Vector3 originalScale;
    private new void Awake()
    {
        base.Awake();
        originalSprite = GetComponent<Image>().sprite;
        originalScale = rectTransform.localScale;
    }

    private new void OnEnable()
    {
        base.OnEnable();
        rectTransform.localScale = originalScale;
    }
    public override void OnMouseEnter()
    {
        if (pauseScaleAnimation) return;
        PlayScaleUpAnimation();
    }
    public override void OnMouseLeave()
    {
        if (pauseScaleAnimation) return;
        ScaleBackToNormal();
    }

    public void ScaleBackToNormal()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(rectTransform, originalScale, animationDuration).setIgnoreTimeScale(true);
    }

    public void PlayScaleUpAnimation()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(rectTransform, new Vector3(scaleFactor, scaleFactor, scaleFactor), animationDuration).setIgnoreTimeScale(true);
    }
    public void ToggleScaleAnimation(bool pause) => pauseScaleAnimation = pause;
}