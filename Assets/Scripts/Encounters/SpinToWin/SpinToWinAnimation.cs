using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpinToWinAnimation : Singleton<SpinToWinAnimation>
{
    [SerializeField] private Image wheel;
    private SpinToWinUI spinToWinUI;
    private Button_Base wheelButton;
    private Sequence buildUpSequence;
    private Vector3 originalPosition;
    public event Action OnSpinComplete;
    public event Action OnSpinStart;
    public event Action OnNotEnoughCoins;
    private bool isSpinning;
    private void Start()
    {
        wheelButton = wheel.GetComponent<Button_Base>();
        spinToWinUI = GetComponent<SpinToWinUI>();
        originalPosition = wheel.rectTransform.anchoredPosition;
        wheelButton.OnInitialPress.AddListener(() =>
        {
            if(isSpinning || spinToWinUI.SpinsLeft == 0) return;
            if(PlayerInventory.Instance.Coins < spinToWinUI.CurrentWager)
            {
                OnNotEnoughCoins?.Invoke();
                return;
            }
            BuildUpAnimation();
        });
        wheelButton.OnRelease.AddListener(() =>
        {
            if (buildUpSequence == null) return;
            StopBuildUpAnimation();
            if (wheelButton.CurrentPressDuration <= 0.3f) return;
            SpinWheel(wheelButton.CurrentPressDuration);
        });
    }

    private void BuildUpAnimation()
    {
        if (buildUpSequence != null && buildUpSequence.IsActive()) return;
        buildUpSequence = DOTween.Sequence()
            .SetLoops(-1, LoopType.Restart);
        
        buildUpSequence.AppendCallback(() =>
        {
            float strength = Mathf.Lerp(1f, 8f, Mathf.Clamp01(wheelButton.CurrentPressDuration / 5f));
            wheel.rectTransform.DOShakeAnchorPos(0.1f, strength);
        });
        
        buildUpSequence.AppendInterval(0.05f);
    }

    private void StopBuildUpAnimation()
    {
        buildUpSequence?.Kill();
        buildUpSequence = null;
        DOTween.Kill(wheel.rectTransform);
        wheel.rectTransform.anchoredPosition = originalPosition;
    }

    private void SpinWheel(float power)
    {
        OnSpinStart?.Invoke();
        isSpinning = true;
        float duration = 2f;
        wheel.rectTransform.DORotate(new Vector3(0, 0, -360 * power * 2f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic).onComplete = () =>
        {
            OnSpinComplete?.Invoke();
            isSpinning = false;
        };
    }
}