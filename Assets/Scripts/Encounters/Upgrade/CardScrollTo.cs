using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CardScrollTo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")] [SerializeField]
    private ScrollRect scrollRect;

    [Header("Animation")] [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private float padding = 60f;

    Tween activeTween;

    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!scrollRect) return;

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport
            ? scrollRect.viewport
            : (RectTransform)scrollRect.transform;

        Vector2 delta = ComputeDelta(scrollRect, viewport, content, (RectTransform)transform, padding);
        if (delta == Vector2.zero) return;

        Vector2 target = content.anchoredPosition + delta;

        activeTween?.Kill();
        activeTween = content
            .DOAnchorPos(target, duration)
            .SetEase(ease)
            .SetUpdate(true); // unscaled time (UI-safe)
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Optional: do nothing, or Kill if you want hover-cancel behavior
        // activeTween?.Kill();
    }

    private static Vector2 ComputeDelta(
        ScrollRect sr,
        RectTransform viewport,
        RectTransform content,
        RectTransform item,
        float pad)
    {
        var vpWorld = new Vector3[4];
        var itWorld = new Vector3[4];

        viewport.GetWorldCorners(vpWorld);
        item.GetWorldCorners(itWorld);

        Vector3 vpMin = content.InverseTransformPoint(vpWorld[0]);
        Vector3 vpMax = content.InverseTransformPoint(vpWorld[2]);
        Vector3 itMin = content.InverseTransformPoint(itWorld[0]);
        Vector3 itMax = content.InverseTransformPoint(itWorld[2]);

        vpMin += new Vector3(pad, pad);
        vpMax -= new Vector3(pad, pad);

        Vector2 delta = Vector2.zero;

        if (sr.horizontal)
        {
            if (itMin.x < vpMin.x)
                delta.x = vpMin.x - itMin.x;
            else if (itMax.x > vpMax.x)
                delta.x = vpMax.x - itMax.x;
        }

        if (sr.vertical)
        {
            if (itMin.y < vpMin.y)
                delta.y = vpMin.y - itMin.y;
            else if (itMax.y > vpMax.y)
                delta.y = vpMax.y - itMax.y;
        }

        return delta;
    }

    public void ScrollToThis(Action OnComplete = null)
    {
        if (!scrollRect) return;

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport
            ? scrollRect.viewport
            : (RectTransform)scrollRect.transform;

        Vector2 delta = ComputeDelta(scrollRect, viewport, content, (RectTransform)transform, padding);
        if (delta == Vector2.zero)
        {
            OnComplete?.Invoke();
            return;
        }
        Vector2 target = content.anchoredPosition + delta;

        activeTween?.Kill();
        activeTween = content
            .DOAnchorPos(target, duration)
            .SetEase(ease)
            .SetUpdate(true).OnComplete(() => OnComplete?.Invoke()); 
    }

    private void OnDisable()
    {
        activeTween?.Kill();
    }
}