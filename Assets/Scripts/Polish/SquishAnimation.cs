using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class SquishAnimation : MonoBehaviour
{
    [SerializeField] private bool setUpdate;
    [SerializeField] private bool playOnAwake;
    [SerializeField] private float strengthMult = 1;
    private Sequence seq;
    private void Start()
    {
        if (playOnAwake)
            StartCoroutine(WaitFrame());
        
        IEnumerator WaitFrame()
        {
            yield return null;
            Squish(strengthMult);
        }
            
    }
    public void Squish(
        float strengthMult = 1f,
        float durationMult = 1f,
        Action OnComplete = null)
    {
        Vector3 baseScale = transform.localScale;
        Vector3 squash = baseScale + (new Vector3(1.2f, 0.7f, 1f) - baseScale) * strengthMult;
        Vector3 stretch = baseScale + (new Vector3(0.9f, 1.1f, 1f) - baseScale) * strengthMult;
        float t1 = 0.08f * durationMult;
        float t2 = 0.10f * durationMult;
        float t3 = 0.08f * durationMult;
        if (seq != null) return;
        seq = DOTween.Sequence().SetUpdate(setUpdate);
        seq.Append(transform.DOScale(squash, t1).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(stretch, t2).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(baseScale, t3).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                seq = null;
                OnComplete?.Invoke();
            });
    }

}
