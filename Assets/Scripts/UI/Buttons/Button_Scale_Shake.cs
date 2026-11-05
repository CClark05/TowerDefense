using DG.Tweening;
using UnityEngine;

public class Button_Scale_Shake : Button_Hover
{
    private Vector3 originalScale;

    private new void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
    }
    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DOScale(originalScale * 1.2f, 0.25f).SetEase(Ease.OutBack);
        transform.DOPunchRotation(new Vector3(0, 0, 2.5f), 0.25f, 6, 0.5f);

    }
    public override void OnMouseLeave()
    {
        base.OnMouseLeave();
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DOScale(originalScale, 0.25f).SetEase(Ease.OutSine);
    }
}
