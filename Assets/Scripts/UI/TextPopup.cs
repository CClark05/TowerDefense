using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float duration = 0.3f;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();

    }

    private void Start()
    {
        text.transform.localScale = Vector3.zero;
        var seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(text.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
        seq.Join(text.transform.DOMoveY(0.7f, 1f).SetRelative().SetEase(Ease.OutCubic));
        seq.AppendInterval(0.2f);
        seq.Append(text.DOFade(0, 0.3f)).OnComplete(() => Destroy(gameObject));
    }

    public void Init(string text)
    {
        this.text.text = text;
    }
    

}
