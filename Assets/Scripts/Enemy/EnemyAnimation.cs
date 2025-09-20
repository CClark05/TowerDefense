using System;
using DG.Tweening;
using UnityEngine;
public class EnemyAnimation : MonoBehaviour
{
    private Vector2 originalScale;
    [SerializeField] private Transform shockwave;
    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        //GetComponent<EnemyMovement>().OnJump += JumpAnimation;
    }
    private void JumpAnimation(float duration)
    {
        var seq = DOTween.Sequence().SetLink(gameObject);
        float squashDuration = duration * 0.2f;
        seq.Append(transform.DOScale(new Vector2(originalScale.x * 1.2f,originalScale.y * 0.8f), squashDuration / 2).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(new Vector2(originalScale.x * 0.9f, originalScale.y * 1.1f), squashDuration / 2));
        seq.AppendInterval(duration - squashDuration);
        seq.Append(transform.DOScale(new Vector3(originalScale.x * 1.16f, originalScale.y * 0.86f), 0.06f)
            .SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale, 0.06f));
        seq.OnComplete(() =>
        {
            transform.localScale = originalScale;
        });
        
    }
}
