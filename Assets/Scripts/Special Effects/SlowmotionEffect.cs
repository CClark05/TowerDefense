using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SlowmotionEffect : MonoBehaviour
{
    public static readonly float freezeFrameDration = 0.3f;
    private void Start()
    {
        Projectile.OnWillKill += () =>
        {
            if(EnemyManager.Instance.CurrentEnemies.Count == 1)
                SlowmoAnimation(0.1f, 0f, 1f);
        };
        EnemyHealth.OnFinalEnemyDeath += () =>
        {
            StartCoroutine(FreezeFrame(freezeFrameDration));
        };
    }

    private void SlowmoAnimation(float to, float toDuration, float holdTime)
    {
        var seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, to, toDuration));
        seq.AppendInterval(holdTime);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, toDuration).SetEase(Ease.OutQuad));
    }

    private IEnumerator FreezeFrame(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}
