using UnityEngine;
using UnityEngine.Serialization;

public class EnemyParticles : MonoBehaviour
{
    [SerializeField] private GameObject deathParticlesPrefab;
    private void Start()
    {
        var baseColor = SpriteColorUtils.AverageColor(GetComponent<SpriteRenderer>().sprite);
        GetComponent<EnemyHealth>().OnDeath += () =>
        {
            if (EnemyManager.Instance.CurrentEnemies.Count != 0) return;
            var particles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var main = particles.main;
            
            Color.RGBToHSV(baseColor, out float h, out float s, out float v);

            float dh = 0.08f;          // hue variation
            float ds = 0.15f;          // saturation variation
            float dv = 0.15f;          // value/brightness variation
            var cMin = Color.HSVToRGB(
                Mathf.Repeat(h - dh, 1f),
                Mathf.Clamp01(s - ds),
                Mathf.Clamp01(v - dv)
            );
            var cMax = Color.HSVToRGB(
                Mathf.Repeat(h + dh, 1f),
                Mathf.Clamp01(s + ds),
                Mathf.Clamp01(v + dv)
            );
            cMin.a = baseColor.a;
            cMax.a = baseColor.a;
            main.startColor = new ParticleSystem.MinMaxGradient(cMin, cMax);
        };
    }
}
