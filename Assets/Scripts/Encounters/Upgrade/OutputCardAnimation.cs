using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class OutputCardAnimation : MonoBehaviour
{
    private Vector3 originalScale;
    private RectTransform rectTransform;
    private void OnEnable()
    {
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        rectTransform.localScale = Vector3.zero;
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return null;
            
            rectTransform.DOScale(originalScale, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                GetComponent<SquishAnimation>().Squish(0.8f, 0.8f);
            });
        }
        

    }
}
