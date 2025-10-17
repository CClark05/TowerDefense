using DG.Tweening;
using UnityEngine;

public static class CameraShake
{
    /// <summary>
    /// Shakes a camera's transform using DOTween.
    /// - Preserves and restores original localPosition/localRotation.
    /// - Kills any prior shake on the same transform (by ID) to prevent stacking drift.
    /// - Links tween lifetime to the GameObject to avoid orphan tweens.
    /// </summary>
    /// <param name="cam">Camera transform to shake (usually Camera.main.transform).</param>
    /// <param name="duration">Seconds to shake.</param>
    /// <param name="posStrength">World units amplitude for position shake.</param>
    /// <param name="vibrato">Roughly number of shakes. Higher = more jagged.</param>
    /// <param name="randomness">Angle variation for shake directions (0–180+).</param>
    /// <param name="fadeOut">Ease out the shake over time.</param>
    /// <param name="rotStrengthDegrees">Optional rotational shake amplitude (degrees). 0 disables rotation shake.</param>
    /// <param name="unscaledTime">True to ignore Time.timeScale.</param>
    /// <returns>The composed tween (Sequence). You can await/chain if desired.</returns>
    public static Tween Shake(
        Transform cam,
        float duration,
        float posStrength,
        int vibrato = 20,
        float randomness = 90f,
        bool fadeOut = true,
        float rotStrengthDegrees = 0f,
        bool unscaledTime = false)
    {
        if (cam == null || duration <= 0f || (posStrength <= 0f && rotStrengthDegrees <= 0f))
            return null;

        // Use a stable tween ID per transform so we can kill/replace on re-entry.
        var tweenId = $"CameraShake::{cam.GetInstanceID()}";
        DOTween.Kill(tweenId);

        // Cache originals to avoid drift (we'll restore on complete/kill).
        Vector3 originalLocalPos = cam.localPosition;
        Quaternion originalLocalRot = cam.localRotation;

        // Build sequence
        var seq = DOTween.Sequence()
            .SetId(tweenId)
            .SetLink(cam.gameObject, LinkBehaviour.KillOnDestroy)
            .SetUpdate(unscaledTime)
            .SetRecyclable(true)
            .OnKill(() =>
            {
                // Ensure exact restore even if killed early.
                cam.localPosition = originalLocalPos;
                cam.localRotation = originalLocalRot;
            })
            .OnComplete(() =>
            {
                cam.localPosition = originalLocalPos;
                cam.localRotation = originalLocalRot;
            });

        if (posStrength > 0f)
        {
            // DOShakePosition applies local shake by default for Transforms.
            seq.Join(cam.DOShakePosition(
                duration: duration,
                strength: posStrength,
                vibrato: vibrato,
                randomness: randomness,
                snapping: false,
                fadeOut: fadeOut));
        }

        if (rotStrengthDegrees > 0f)
        {
            seq.Join(cam.DOShakeRotation(
                duration: duration,
                strength: new Vector3(rotStrengthDegrees, rotStrengthDegrees, rotStrengthDegrees),
                vibrato: vibrato,
                randomness: randomness,
                fadeOut: fadeOut));
        }

        return seq;
    }

    public static Tween ShakeDefault()
    {
        return Shake(Camera.main.transform, 0.2f, 0.4f);
    }
}