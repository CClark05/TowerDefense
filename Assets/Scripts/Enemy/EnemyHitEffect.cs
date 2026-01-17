using System;
using CodeMonkey.Utils;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float flashAmountOnHit = 1f;
    [SerializeField] private float flashToDuration = 0.06f;
    [SerializeField] private float holdDuration = 0.06f;
    [SerializeField] private float flashBackDuration = 0.18f;
    private static readonly int FlashProp = Shader.PropertyToID("_Flash");
    private SpriteRenderer sr;
    private Sequence flashSeq;
    private Material material;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        material = sr.material;
    }

    private void Start()
    {
        GetComponent<EnemyHealth>().OnHit += () => Flash(1);
        EnemyHealth.OnDeathStatic += OnDeathStatic;
    }

    private void OnDeathStatic(bool final)
    {
        if (final)
        {
            Flash(7);
        }
    }

    /**
    private void HitEffect()
    {
        flashSeq?.Kill();
        flashSeq = DOTween.Sequence(gameObject).SetLink(gameObject).SetUpdate(true);

        Tween tweenTo(float to, float duration)
        {
            return DOTween.To(() => material.GetFloat(FlashProp), x =>
            {
                material.SetFloat(FlashProp, x);
            }, to, duration);
        }
        
        flashSeq.Append(tweenTo(1, flashToDuration));
        flashSeq.AppendInterval(holdDuration);
        flashSeq.Append(tweenTo(0, flashBackDuration));
    }
    */
    private void Flash(int times)
    {
        flashSeq?.Kill();
        flashSeq = DOTween.Sequence(gameObject).SetLink(gameObject).SetUpdate(true);
        float From() => material.GetFloat(FlashProp);
        void Setter(float v) => material.SetFloat(FlashProp, v);
        for (int i = 0; i < times; i++)
        {
            flashSeq.Append(DOTween.To(From, Setter, flashAmountOnHit, flashToDuration))
                .AppendInterval(holdDuration)
                .Append(DOTween.To(From, Setter, 0f, flashBackDuration));
        }
    }

    private void OnDestroy()
    {
        EnemyHealth.OnDeathStatic -= OnDeathStatic;
    }
}
