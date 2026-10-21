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

    [SerializeField] private SpriteRenderer overlaySr;
    private static readonly int FlashProp = Shader.PropertyToID("_Flash");
    private SpriteRenderer sr;
    private Sequence flashSeq;
    private Material material;
    private Material overlayMaterial;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        overlayMaterial = overlaySr.material;
        material = sr.material;
    }

    private void Start()
    {
        GetComponent<EnemyHealth>().OnHit += () => Flash(1);
    }


    private void Flash(int times)
    {
        flashSeq?.Kill();
        flashSeq = DOTween.Sequence(gameObject).SetLink(gameObject).SetUpdate(true);
        float From() => material.GetFloat(FlashProp);
        void Setter(float v)
        {
            overlayMaterial.SetFloat(FlashProp, v);
            material.SetFloat(FlashProp, v);
        }

        for (int i = 0; i < times; i++)
        {
            flashSeq.Append(DOTween.To(From, Setter, flashAmountOnHit, flashToDuration))
                .AppendInterval(holdDuration)
                .Append(DOTween.To(From, Setter, 0f, flashBackDuration));
        }
    }
    
}
