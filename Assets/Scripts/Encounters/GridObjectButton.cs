using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GridObjectButton : MonoBehaviour, IHoverable
{
    public SOEvent OnClickObject;
    private Vector3 originalScale;
    private void Awake()
    {
        originalScale = transform.localScale;
    }
    public void OnHover()
    {
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutSine);
        transform.DOPunchRotation(new Vector3(0, 0, 5f), 0.2f);
    }
    public void OnLeaveHover()
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutSine);
    }
    public void OnClick()
    {
        OnClickObject.Raise(this);
    }
    
}
