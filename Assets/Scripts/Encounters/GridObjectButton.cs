using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GridObjectButton : MonoBehaviour, IHoverable
{
    public SOEvent OnClickObject;
    private Vector3 originalScale;
    private Sequence seq;
    private void Awake()
    {
        originalScale = transform.localScale;
    }
    public void OnHover()
    {
        if (seq != null) return;

        seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutSine));
        seq.Join(transform.DOPunchRotation(new Vector3(0, 0, 5f), 0.2f));
        seq.OnComplete(() => seq = null); 
    }
    public void OnLeaveHover()
    {
        seq?.Kill();
        seq = null;
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutSine);
        transform.localRotation = Quaternion.identity;
    }
    public void OnClick()
    {
        OnClickObject.Raise(this);
    }
    
}
