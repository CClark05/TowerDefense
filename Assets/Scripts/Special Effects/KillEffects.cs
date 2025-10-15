using System.Collections;
using DG.Tweening;
using UnityEngine;

public class KillEffects : MonoBehaviour
{
    public static readonly float finalFreezeFrameDration = 0.3f;
    public static readonly float freezeFrameDuration = 0.075f;
    private void Start()
    {
        Projectile.OnWillKill += () =>
        {
            if(EnemyManager.Instance.CurrentEnemies.Count == 1 && EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning)
                SlowmoAnimation(0.1f, 0f, 1f);
        };
        EnemyHealth.OnDeathStatic += (bool final) =>
        {
            if(final)
                SlowmoAnimation(0.1f, 0, 1f);
            StartCoroutine(FreezeFrame(final ? finalFreezeFrameDration : freezeFrameDuration));
        };
    }

    private void SlowmoAnimation(float to, float toDuration, float holdTime)
    {
        Debug.Log("slow mo");
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
