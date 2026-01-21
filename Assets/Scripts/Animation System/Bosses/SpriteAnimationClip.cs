using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite Animation Clip", menuName = "Animation Clips/Sprite")]
public class SpriteAnimationClip : AnimationClip
{
    public Sprite[] sprites;
    public override IEnumerator Play(Transform transform, float duration, Func<bool> cancelled, AnimArgs args = null)
    {
        (args ??= new AnimArgs()).fps ??= 10f;
        float frameDuration = 1f / args.fps.Value;
        SpriteRenderer sr = transform.GetComponent<SpriteRenderer>();
        Sprite originalSprite = sr.sprite;
        do
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (cancelled())
                {
                    sr.sprite = originalSprite;
                    yield break;
                }

                sr.sprite = sprites[i % sprites.Length];
                yield return new WaitForSeconds(frameDuration);
            }
        } while (looping);

        sr.sprite = originalSprite;
    }
    public void ApplyLastFrame(Transform transform, AnimArgs args)
    {
        var sr = transform.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[^1];
    }
}