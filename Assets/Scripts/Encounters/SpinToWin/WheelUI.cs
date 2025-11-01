using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelUI : MonoBehaviour
{
    [SerializeField] private GameObject multTextPrefab;
    [SerializeField] private GameObject textContainer;
    [SerializeField] private SpinToWinAnimation wheelAnimation;
    [SerializeField] private Image markerImage;
    private RectTransform wheelRectTransform => GetComponent<RectTransform>();
    private float textRadiusPercent = 0.7f;
    private TextMeshProUGUI[] textObjects;

    private float[] multipliers =
    {
        0, 0, 0.5f, 0.5f, 0.5f, 1, 1, 1, 1.2f, 1.2f,
        1.5f, 1.5f, 2, 2, 3, 7
    };

    public Dictionary<int, Color> segmentColors { get; private set; } = new();
    public float CurrentMult => multipliers[GetCurrentSegment(wheelRectTransform.eulerAngles.z)];
    public Color CurrentColor => segmentColors[GetCurrentSegment(wheelRectTransform.eulerAngles.z)];
    private void Start()
    {
        float angleStep = 360f / multipliers.Length;
        float textRadius = (GetComponent<RectTransform>().rect.width / 2f) * textRadiusPercent;
        multipliers  = multipliers.OrderBy(_ => UnityEngine.Random.value).ToArray();
        textObjects = new TextMeshProUGUI[multipliers.Length];
        for (int i = 0; i < multipliers.Length; i++)
        {
            
            TextMeshProUGUI text = Instantiate(multTextPrefab, textContainer.transform).GetComponent<TextMeshProUGUI>();
            text.text = $"{multipliers[i]}x";
            float angle = 90f - (i * angleStep) - (angleStep / 2f);
            float radians = angle * Mathf.Deg2Rad;

            Vector2 position = new Vector2(
                Mathf.Cos(radians) * textRadius,
                Mathf.Sin(radians) * textRadius
            );

            text.rectTransform.anchoredPosition = position;
            textObjects[i] = text;
            text.color = new Color(1, 1, 1, 0.5f);
            segmentColors[i] = GetPixelColor.SampleColorAtTransform(GetComponent<Image>(),text.rectTransform);
            markerImage.color = segmentColors[GetCurrentSegment(wheelRectTransform.eulerAngles.z)];
        }

        UpdateUI();
        Coroutine updateCoroutine = null;
        wheelAnimation.OnSpinStart += () => { updateCoroutine ??= StartCoroutine(UpdateUIRoutine()); };
        wheelAnimation.OnSpinComplete += () =>
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        };
    }

    private IEnumerator UpdateUIRoutine()
    {
        while (true)
        {
            UpdateUI();
            yield return null;
        }
    }

    private void UpdateUI()
    {
        int currentSegment = GetCurrentSegment(wheelRectTransform.eulerAngles.z);
        for (int i = 0; i < textObjects.Length; i++)
        {
            if (i == currentSegment)
            {
                textObjects[i].color = Color.white;
                textObjects[i].fontSize = 26;
            }
            else
            {
                textObjects[i].color = new Color(1, 1, 1, 0.5f);
                textObjects[i].fontSize = 23;
            }
        }
        
        markerImage.color = segmentColors[GetCurrentSegment(wheelRectTransform.eulerAngles.z)];
    }

    private int GetCurrentSegment(float wheelRotation)
    {
        float angleStep = 360f / multipliers.Length;
        float adjustedRotation = (wheelRotation) % 360f;
        if (adjustedRotation < 0) adjustedRotation += 360f;
        int segment = Mathf.FloorToInt(adjustedRotation / angleStep);
        segment %= multipliers.Length;
        return segment;
    }
}