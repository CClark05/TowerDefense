using System.Collections;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    [SerializeField] private float duration, strength;
    [SerializeField] private int vibrato;
    public IEnumerator Shake()
    {
        RectTransform rect = transform as RectTransform;
        Vector3 originalPos = rect.anchoredPosition;

        for (int i = 0; i < vibrato; i++)
        {
            float progress = (float)i / vibrato;
            float damper = 1f - progress; // fade out
            float offsetX = Random.Range(-1f, 1f) * strength * damper;
            float offsetY = Random.Range(-0.5f, 0.5f) * strength * 0.25f * damper; // subtle vertical

            rect.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            yield return new WaitForSeconds(duration / vibrato);
        }

        rect.anchoredPosition = originalPos; // reset
    }
    
    public void TriggerShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }
}