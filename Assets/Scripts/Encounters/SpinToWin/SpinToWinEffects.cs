using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpinToWinEffects : MonoBehaviour
{
    [SerializeField] private GameObject multPopPrefab;
    [SerializeField] private WheelUI wheelUI;
    private SpinToWinUI spinToWin;

    private void Awake()
    {
        spinToWin = GetComponent<SpinToWinUI>();
        spinToWin.OnWagerComplete += _ =>
        {
            if (wheelUI.CurrentMult <= 1) return;
            var popup = Instantiate(multPopPrefab, spinToWin.transform.GetChild(0)).GetComponent<TextMeshProUGUI>();
            popup.text = $"x{wheelUI.CurrentMult}";
            popup.color = wheelUI.CurrentColor;

            float targetScale = 1f + Mathf.Log(wheelUI.CurrentMult + 1f, 2f) * 0.3f;
            float intensity = Mathf.Clamp01(wheelUI.CurrentMult / 8f);

            Sequence popupSequence = DOTween.Sequence();
            popupSequence.Append(popup.transform.DOScale(targetScale, 0.15f).SetEase(Ease.OutCubic));
            popupSequence.AppendCallback(() => { popup.transform.DOShakeRotation(0.3f, new Vector3(0, 0, 30), 15, 45); });
            popupSequence.Join(popup.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.3f, 10, 0.5f).SetEase(Ease.InOutSine));

            Vector3 finalScale = popup.transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
            popupSequence.Insert(0.35f, popup.transform.DOScale(finalScale, 1f).SetEase(Ease.OutQuad));

            Vector2 moveOffset = new Vector2(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(0f, 1f)
            );
            popupSequence.Join(popup.transform.DOLocalMove(moveOffset, UnityEngine.Random.Range(0.4f, 0.6f))
                .SetRelative(true).SetEase(Ease.OutCubic));

            float fadeDuration = 1.25f;
            popupSequence.Insert(0.95f, popup.DOFade(0, fadeDuration).SetEase(Ease.OutSine));
            if (wheelUI.CurrentMult >= 7f)
            {
                Color[] colors = wheelUI.segmentColors.Values.ToArray();
                float colorDuration = (fadeDuration / colors.Length) * 0.5f;
                for (int i = 0; i < colors.Length; i++)
                {
                    Color fromColor = i == 0 ? wheelUI.CurrentColor : colors[i - 1];
                    Color toColor = colors[i];
                    popupSequence.Insert(0.95f + (i * colorDuration),
                        DOTween.To(() => popup.color, x => { popup.color = new Color(x.r, x.g, x.b, popup.color.a); }, toColor, colorDuration));
                }
            }

            popupSequence.OnComplete(() => Destroy(popup.gameObject));
        };
    }
}