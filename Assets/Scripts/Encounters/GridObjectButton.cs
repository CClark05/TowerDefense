using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GridObjectButton : MonoBehaviour, IHoverable
{
    public SOEvent OnClickObject;
    private Vector3 originalScale;
    private Sequence seq;
    private WhiteOutlineEffect outlineEffect;
    private void Awake()
    {
        originalScale = transform.localScale;
        outlineEffect = GetComponent<WhiteOutlineEffect>();
    }
    public void OnHover()
    {
        GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DOScale(originalScale * 1.2f, 0.25f).SetEase(Ease.OutBack);
        transform.DOPunchRotation(new Vector3(0, 0, 2.5f), 0.25f, 6, 0.5f);
        outlineEffect.ToggleWhiteOutline(true);
    }
    public void OnLeaveHover()
    {
        outlineEffect.ToggleWhiteOutline(false);
        GameManager.Instance.SetCursor(GameManager.Cursors.Default);
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DOScale(originalScale, 0.25f).SetEase(Ease.OutSine);
    }
    public void OnClick()
    {
        OnClickObject.Raise(this);
    }
    
}
