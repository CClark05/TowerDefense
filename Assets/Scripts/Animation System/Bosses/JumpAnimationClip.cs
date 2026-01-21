using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "JumpAnimationClip", menuName = "Animation Clips/Boss/Jump Animation Clip")]
public class JumpAnimationClip : AnimationClip
{
    [Range(0,1)] public float squashPortion = 0.2f;
    public Vector2 squashScale = new(1.2f, 0.8f);
    public Vector2 stretchScale = new(0.9f, 1.1f);
    public float impactTime = 0.06f;
    
    public override IEnumerator Play(Transform transform, float duration, Func<bool> cancelled, AnimArgs args = null)
    {
        var seq = DOTween.Sequence().SetLink(transform.gameObject);
        float squashDuration = duration * 0.2f;
        Vector2 originalScale = transform.localScale;
        seq.Append(transform.DOScale(new Vector2(originalScale.x * squashScale.x,originalScale.y * squashScale.y), squashDuration / 2).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(new Vector2(originalScale.x * stretchScale.x, originalScale.y * stretchScale.y), squashDuration / 2));
        seq.AppendInterval(duration - squashDuration);
        seq.Append(transform.DOScale(new Vector3(originalScale.x * squashScale.x, originalScale.y * squashScale.y), 0.06f)
            .SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale, impactTime));
        seq.OnComplete(() =>
        {
            transform.localScale = originalScale;
        });
        yield return new DOTweenCYInstruction.WaitForCompletion(seq);
    }
    
}