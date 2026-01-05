using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ColorAnimation : MonoBehaviour
{
    [SerializeField] private Color colorA;
    [SerializeField] private Color colorB;

    [Tooltip("Time to go from A → B")]
    [SerializeField] private float cycleDuration = 0.5f;

    [Tooltip("<= 0 means infinite")]
    [SerializeField] private float totalDuration;

    private Image image;
    private Tween colorTween;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    [SerializeField] private float holdDuration = 0.2f; 

    private void OnEnable()
    {
        image.color = colorA;

        var seq = DOTween.Sequence();

        seq.Append(image.DOColor(colorB, cycleDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(holdDuration);
        seq.Append(image.DOColor(colorA, cycleDuration).SetEase(Ease.InOutSine));
        seq.AppendInterval(holdDuration);

        seq.SetLoops(-1);
        seq.SetLink(gameObject);

        colorTween = seq;

        if (totalDuration > 0f)
        {
            DOVirtual.DelayedCall(totalDuration, StopAnimation)
                .SetLink(gameObject);
        }
    }


    private void OnDisable()
    {
        StopAnimation();
    }

    private void StopAnimation()
    {
        colorTween?.Kill();
        colorTween = null;
        image.color = colorA; 
    }
}