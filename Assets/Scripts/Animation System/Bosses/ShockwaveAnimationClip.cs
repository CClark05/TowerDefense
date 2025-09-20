using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ShockwaveClip", menuName = "Animation Clips/Boss/Shockwave Animation Clip")]
public class ShockwaveAnimationClip : AnimationClip
{
    //TODO ADD OPTIONAL PARAMETERS
    public override IEnumerator Play(Transform transform, float duration, Func<bool> cancelled)
    {
        var seq = DOTween.Sequence().SetLink(transform.gameObject);
        seq.Append(transform.DOScale(new Vector2(0.6f,0.6f), duration).SetEase(Ease.OutQuint));
        seq.Join(transform.GetComponent<SpriteRenderer>().DOFade(0, duration * 1.5f).SetEase(Ease.OutQuad));
        seq.OnComplete(() =>
        {
            transform.localScale = Vector2.zero;
            var sr = transform.GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        });
        yield return new DOTweenCYInstruction.WaitForCompletion(seq);    
    }
}