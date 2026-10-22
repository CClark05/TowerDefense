using System;
using DG.Tweening;
using UnityEngine;

public class TowerShopButtonAnimation : MonoBehaviour
{
    private TowerShopButton button;
    private Vector3 originalScale;
    private void Start()
    {
        button = GetComponent<TowerShopButton>();
        button.OnHover += AnimateHover;
        button.OnLeaveHover += AnimateLeaveHover;
        originalScale = transform.localScale;
    }
    private void AnimateHover()
    {
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        transform.DOScale(originalScale * 1.2f, 0.25f).SetEase(Ease.OutBack);
        transform.DOPunchRotation(new Vector3(0, 0, 2.5f), 0.25f, 6, 0.5f);
    }
    private void AnimateLeaveHover()
    {
        transform.DOScale(originalScale, 0.25f).SetEase(Ease.OutSine);
    }

  
}
