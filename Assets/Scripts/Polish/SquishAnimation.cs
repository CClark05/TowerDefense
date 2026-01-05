using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class SquishAnimation : MonoBehaviour
{
    [SerializeField] private bool setUpdate;
    [SerializeField] private bool playOnAwake;
    [SerializeField] private float strengthMult = 1;
    private void Awake()
    {
        if (playOnAwake)
            Squish(strengthMult);
    }
    public void Squish(float strengthMult = 1, Action OnComplete = null)
    {
        Vector3 baseScale = Vector3.one;

        Vector3 squash = baseScale + (new Vector3(1.2f, 0.7f, 1f) - baseScale) * strengthMult;
        Vector3 stretch = baseScale + (new Vector3(0.9f, 1.1f, 1f) - baseScale) * strengthMult;

        Sequence seq = DOTween.Sequence().SetUpdate(setUpdate);
        seq.Append(transform.DOScale(squash, 0.08f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(stretch, 0.10f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(baseScale, 0.08f).SetEase(Ease.OutBack)).OnComplete(() => OnComplete?.Invoke());
    }
}
