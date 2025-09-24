using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TowerAnimation : MonoBehaviour
{
    [SerializeField] private GameObject effectTextPrefab;
    [SerializeField] private Transform effectTextParent;
    private SkillContext skillContext;
    private void Start()
    {
        skillContext = GetComponent<TowerDataHolder>().SkillContext;
        skillContext.OnBuffAdded += OnBuffAdded;
    }

    private void OnBuffAdded(IBuff buff, int stacks)
    {
        var text = Instantiate(effectTextPrefab, effectTextParent).GetComponent<TextMeshProUGUI>();
        text.text = (buff as EffectData).name;
        text.color = (buff as EffectData).color;
        var rect = text.rectTransform;
        rect.localScale = Vector2.zero;
        var seq = DOTween.Sequence().SetLink(rect.gameObject);
        Vector2 scaleTo = new Vector2(2, 2);
        float scaleDuration = 0.5f;
        seq.Append(rect.DOScale(scaleTo, scaleDuration).SetEase(Ease.OutBack, overshoot: 2));
        seq.Join(text.DOFade(0, scaleDuration).From());
        seq.Insert(0.3f,rect.DOLocalMove(new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(2f, 4f)),
            1f).SetRelative(true).SetEase(Ease.OutCubic));
        seq.AppendInterval(0.3f);
        seq.Append(text.DOFade(0, 0.5f)).onComplete += () =>
        {
            Destroy(rect.gameObject);
        };
        
    }
}