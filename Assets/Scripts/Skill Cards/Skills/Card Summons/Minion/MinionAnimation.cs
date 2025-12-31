using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class MinionAnimation : MonoBehaviour
{
    [SerializeField] private AnimationClip blinkAnimation;
    private IAnimationPlayer animationPlayer;
    private float timer;
    private float randomBlinkInterval;

    private void Awake()
    {
        animationPlayer = GetComponent<IAnimationPlayer>();
        randomBlinkInterval = UnityEngine.Random.Range(8f, 18f);
        transform.localScale = Vector2.zero;
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(SquishAnimation);
        GetComponent<Minion>().OnShoot += SquishAnimation;
        GetComponent<Minion>().OnDestroy += () =>
        {
            transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        };
    }



    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= randomBlinkInterval)
        {
            timer = 0;
            randomBlinkInterval = UnityEngine.Random.Range(8f, 18f);
            animationPlayer.Play(blinkAnimation, transform);
        }
    }

    private void SquishAnimation()
    {
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(transform.DOScale(new Vector3(1.2f, 0.7f, 1f), 0.08f)
                .SetEase(Ease.OutQuad))
            .Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.10f)
                .SetEase(Ease.OutQuad))
            .Append(transform.DOScale(Vector3.one, 0.08f)
                .SetEase(Ease.OutBack));
    }
   
}