using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class KillEffects : MonoBehaviour
{
    public static readonly float freezeFrameDuration = 0.075f;
    private Coroutine freezeFrameCoroutine;
    private void Start()
    {
        EnemyHealth.OnDeathStatic += OnDeath;

    }
    void OnDeath(bool final)
    {
        if (!final) return;
        if (freezeFrameCoroutine != null) return;
        freezeFrameCoroutine = StartCoroutine(FreezeFrame(freezeFrameDuration, final));
    }
    private void SlowmoAnimation(float to, float toDuration, float holdTime)
    {
        var seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, to, toDuration)).SetEase(Ease.InCubic);
        seq.AppendInterval(holdTime);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, toDuration).SetEase(Ease.OutQuad));
    }

    private IEnumerator FreezeFrame(float duration, bool final)
    {
        var originalTimeScale = final ? 1f : Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
        freezeFrameCoroutine = null;
    }

    private void OnDisable()
    {
        EnemyHealth.OnDeathStatic -= OnDeath;
    }
}