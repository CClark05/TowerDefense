using System;
using DG.Tweening;
using UnityEngine;

public class CardIconAnimation : MonoBehaviour
{
    private CardIconUI iconUI;
    private TowerDataHolder towerDataHolder;
    private Vector3 originalScale;
    private void Awake()
    {
        iconUI = GetComponent<CardIconUI>();
    }

    private void Start()
    {
        towerDataHolder = GetComponentInParent<TowerDataHolder>();
        towerDataHolder.SkillContext.OnCardPlayed += OnCardPlayed;
        originalScale = GetComponent<RectTransform>().localScale;
    }

    private void OnCardPlayed(SkillInstance data)
    {
        float scaleFactor = 0.25f;
        float duration = 0.3f;
        if (data != iconUI.SkillInstance) return;
        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOPunchScale(Vector3.one * scaleFactor, duration, 10, 1f)
            .OnComplete(() => transform.localScale = originalScale);
    }

    private void OnDestroy()
    {
        towerDataHolder.SkillContext.OnCardPlayed -= OnCardPlayed;
    }
}
