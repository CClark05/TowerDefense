using System.Collections;
using DG.Tweening;
using UnityEngine;

public class KillEffects : MonoBehaviour
{
    public static readonly float finalFreezeFrameDuration = 0.3f;
    public static readonly float freezeFrameDuration = 0.075f;
    private void Start()
    {
        /**
        Projectile.OnWillKill += () =>
        {
            if(EnemyManager.Instance.CurrentEnemies.Count == 1 && EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning)
                SlowmoAnimation(0.1f, 0f, 1f);
        };
        */
        EnemyHealth.OnDeathStatic += (bool final) =>
        {
            if(final)
                SlowmoAnimation(0.2f, 0.2f, 0.5f);
            StartCoroutine(FreezeFrame(final ? finalFreezeFrameDuration : freezeFrameDuration));
        };
        /**
        EnemyManager.Instance.OnIdle += () =>
        {
            Time.timeScale = 1;
        };
        */
    }

    private void SlowmoAnimation(float to, float toDuration, float holdTime)
    {
        var seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, to, toDuration)).SetEase(Ease.InCubic);
        seq.AppendInterval(holdTime);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, toDuration).SetEase(Ease.OutQuad));
    }

    private IEnumerator FreezeFrame(float duration)
    {
        float originalTimeScale = Time.timeScale >= 1 ? Time.timeScale : 1f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }
}
