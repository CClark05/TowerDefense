using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveCardOverlap : MonoBehaviour
{
    [Range(0f, 1f)] public float maxRevealFracFewCards = 0.92f;
    [Range(0f, 1f)] public float minRevealFracMany = 0.24f;
    public int maxCardCountForTuning = 8;
    public float extraTop = 8f, extraBottom = 8f;

    [SerializeField] RectTransform playButton;
    [SerializeField] float playButtonGap = 8f;

    VerticalLayoutGroup vlg;
    RectTransform rt;

    void Awake()
    {
        vlg = GetComponent<VerticalLayoutGroup>();
        rt = (RectTransform)transform;
    }

    void OnEnable() => StartCoroutine(ApplyNextFrame());
    void OnRectTransformDimensionsChange() => StartCoroutine(ApplyNextFrame());
    void OnTransformChildrenChanged() => StartCoroutine(ApplyNextFrame());

    System.Collections.IEnumerator ApplyNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        Apply();
    }

    void Apply()
    {
        int n = 0;
        float H = 0f;
        for (int i = 0; i < rt.childCount; i++)
        {
            var go = rt.GetChild(i).gameObject;
            if (!go.activeInHierarchy) continue;
            n++;
            var r = (RectTransform)rt.GetChild(i);
            float ph = LayoutUtility.GetPreferredHeight(r);
            if (ph <= 0f) ph = r.rect.height;
            if (ph > H) H = ph;
        }

        if (n <= 1 || H <= 0f)
        {
            vlg.spacing = 0f;
            return;
        }

        float avail = rt.rect.height - vlg.padding.vertical - extraTop - extraBottom;

        if (playButton)
        {
            var a = new Vector3[4]; var b = new Vector3[4];
            rt.GetWorldCorners(a);
            playButton.GetWorldCorners(b);

            float bottomY = a[0].y;
            float limitY = b[1].y - playButtonGap;
            float extra = Mathf.Max(0f, limitY - bottomY);
            if (extra > 0f) avail += extra;
        }

        float stepNeeded = (avail - H) / (n - 1);
        float t = Mathf.InverseLerp(2f, maxCardCountForTuning, Mathf.Clamp(n, 2, maxCardCountForTuning));
        float desired = Mathf.Lerp(maxRevealFracFewCards, minRevealFracMany, t) * H;
        float visible = Mathf.Clamp(Mathf.Min(stepNeeded, desired), minRevealFracMany * H, maxRevealFracFewCards * H);

        vlg.spacing = visible - H;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}