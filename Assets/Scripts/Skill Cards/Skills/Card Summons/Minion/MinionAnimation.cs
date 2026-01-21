using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class MinionAnimation : MonoBehaviour
{
    [SerializeField] private AnimationClip blinkAnimation, closeEyesAnimation, openEyesAnimation;
    private IAnimationPlayer animationPlayer;
    private float timer;
    private float randomBlinkInterval;
    private Sprite startingSprite;
    [SerializeField] private Sprite blinkSprite;
    private SpriteRenderer sr;
    private Minion minion;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animationPlayer = GetComponent<IAnimationPlayer>();
        minion = GetComponent<Minion>();
        randomBlinkInterval = UnityEngine.Random.Range(8f, 18f);
        transform.localScale = Vector2.zero;
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
        {
            SquishAnimation();
            animationPlayer.Play(openEyesAnimation, transform);
        });
        startingSprite = sr.sprite;
        sr.sprite = blinkSprite;
        GetComponent<Minion>().OnShoot += SquishAnimation;
        GetComponent<Minion>().OnDestroy += () => { transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => { Destroy(gameObject); }); };
        float timer = 0;
        Coroutine countRoutine = null;
        GetComponent<Minion>().OnEnemyEnteredRange += () =>
        {
            if (countRoutine != null)
            {
                StopCoroutine(countRoutine);
                countRoutine = null;
                if (sr.sprite == blinkSprite)
                    animationPlayer.Play(openEyesAnimation, transform);
            }

            timer = 0;
        };
        GetComponent<Minion>().OnNoTarget += () =>
        {
            if (countRoutine != null) return;
            countRoutine = StartCoroutine(Count());
        };
        
        IEnumerator Count()
        {
            while (true)
            {
                if (minion.Target != null)
                {
                    timer = 0f;
                    countRoutine = null;
                    yield break;
                }
                timer += Time.unscaledDeltaTime;
                if(timer >= 3f)
                {
                    timer = 0;
                    animationPlayer.Play(closeEyesAnimation, transform);
                    countRoutine = null;
                    break;
                }
                yield return null;
            }
        }
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