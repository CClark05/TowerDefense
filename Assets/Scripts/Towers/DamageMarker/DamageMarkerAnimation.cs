using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageMarkerAnimation : MonoBehaviour
{
    public event Action OnDie;
    private void Start()
    {
        transform.DOLocalMove(GetComponent<DamageMarker>().RandomPosition, UnityEngine.Random.Range(0.4f, 0.6f)).SetRelative(true).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            GetComponent<TextMeshPro>().DOFade(0, 0.3f).SetDelay(0.6f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                OnDie?.Invoke();
                Destroy(gameObject);
            });
        });
        transform.DOScale(Vector3.zero, 0.25f).From().SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Vector3 fromScale = transform.localScale;
            if (GetComponent<DamageMarker>().AddPunchEffect)
            {
                transform.DOShakeRotation(0.3f, new Vector3(0, 0, 30), 15, 45);
                transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.3f, 10, 0.5f).SetEase(Ease.InOutSine).SetRelative(true);
            }

            transform.DOScale(fromScale + new Vector3(0.1f, 0.1f, 0.1f), 1).SetEase(Ease.OutQuad).SetDelay(0.2f);
        });
    }
}