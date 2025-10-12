using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public class VignetteController : Singleton<VignetteController>
{
    [SerializeField] private Volume volume; // Assign your Global Volume

    private Vignette _vignette;

    // Active tweens (so we can kill/replace safely)
    private Tween _intensityTw;
    private Tween _smoothnessTw;
    private Sequence _fadeSeq;

    public float Intensity => _vignette?.intensity.value ?? 0f;
    public float Smoothness => _vignette?.smoothness.value ?? 0f;

    new void Awake()
    {
        base.Awake();
        if (volume == null)
        {
            volume = GetComponent<Volume>();
            if (volume == null)
            {
                Debug.LogError("VignetteController: No Volume assigned or found.");
                return;
            }
        }

        if (volume.profile == null)
        {
            Debug.LogError("VignetteController: Volume has no profile.");
            return;
        }

        if (!volume.profile.TryGet(out _vignette))
            _vignette = volume.profile.Add<Vignette>(true);
    }

    public void SetEnabled(bool enabled) => volume.weight = enabled ? 1f : 0f;

    public void SetIntensity(float value)
    {
        if (_vignette == null) return;
        _vignette.intensity.Override(Mathf.Clamp01(value));
    }

    public void SetSmoothness(float value)
    {
        if (_vignette == null) return;
        _vignette.smoothness.Override(Mathf.Clamp01(value));
    }

    public void SetColor(Color color)
    {
        if (_vignette == null) return;
        _vignette.color.Override(color);
    }

    public void SetCenter(Vector2 uv)
    {
        if (_vignette == null) return;
        _vignette.center.Override(uv);
    }

    /// <summary>
    /// Tweens intensity & smoothness together.
    /// </summary>
    /// <param name="targetIntensity">0..1</param>
    /// <param name="targetSmoothness">0..1</param>
    /// <param name="duration">seconds</param>
    /// <param name="ease">DOTween ease</param>
    /// <param name="unscaledTime">true = ignores Time.timeScale</param>
    public Sequence FadeTo(float targetIntensity, float targetSmoothness, float duration,
                           Ease ease = Ease.OutQuad, bool unscaledTime = true)
    {
        if (_vignette == null) return null;

        KillTweens();

        _fadeSeq = DOTween.Sequence().SetEase(ease).SetUpdate(unscaledTime);

        _intensityTw = DOTween
            .To(() => Intensity, SetIntensity, Mathf.Clamp01(targetIntensity), duration);

        _smoothnessTw = DOTween
            .To(() => Smoothness, SetSmoothness, Mathf.Clamp01(targetSmoothness), duration);

        _fadeSeq.Join(_intensityTw).Join(_smoothnessTw);
        return _fadeSeq;
    }

    /// <summary>
    /// Quick “pulse” on intensity (up then back).
    /// </summary>
    public Sequence Pulse(float addIntensity, float each = 0.15f, bool unscaledTime = true)
    {
        if (_vignette == null) return null;
        KillTweens();

        float from = Intensity;
        float to = Mathf.Clamp01(from + addIntensity);

        _fadeSeq = DOTween.Sequence().SetUpdate(unscaledTime).SetEase(Ease.OutQuad);
        _fadeSeq.Append(DOTween.To(() => Intensity, SetIntensity, to, each));
        _fadeSeq.Append(DOTween.To(() => Intensity, SetIntensity, from, each));
        return _fadeSeq;
    }

    /// <summary>Stops any running vignette tweens safely.</summary>
    public void KillTweens()
    {
        _intensityTw?.Kill();
        _smoothnessTw?.Kill();
        _fadeSeq?.Kill();
        _intensityTw = _smoothnessTw = null;
        _fadeSeq = null;
    }
}