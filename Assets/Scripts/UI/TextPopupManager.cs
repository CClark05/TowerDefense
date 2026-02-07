using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPopupManager : Singleton<TextPopupManager>
{
     [SerializeField] private GameObject textPopupPrefab;

    private Canvas canvas;
    private List<TextPopup> activePopups = new();
    
    private readonly Dictionary<Vector2Int, Queue<(string text, Vector3 pos, float dur)>> popupQueues = new();
    private readonly HashSet<Vector2Int> runningQueues = new();

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        SkillInstance.OnDisposeSkill += (instance, position) =>
        {
            string text = $"<b><size=125%>{instance.Data.name}</size></b> <color=#{ColorUtility.ToHtmlStringRGB(ColorPicker.red)}>Destroyed</color>";
            QueueTextPopup(text, position + new Vector2(5, 0), 0.5f);
        };
        SkillInstance.OnUpgradeSkill += (instance, position) =>
        {
            string text = $"<b><size=125%>{instance.Data.name}</size></b> <color=#{ColorUtility.ToHtmlStringRGB(ColorPicker.green)}>Upgraded</color>";
            QueueTextPopup(text, position + new Vector2(5, 0), 0.5f);
        };
    }
    
    public void QueueTextPopup(string text, Vector3 position, float durationIncrease = 0f)
    {
        Vector2Int key = new(
            Mathf.RoundToInt(position.x * 2f),
            Mathf.RoundToInt(position.y * 2f)
        );

        if (!popupQueues.TryGetValue(key, out var queue))
            popupQueues[key] = queue = new();

        queue.Enqueue((text, position, durationIncrease));

        if (runningQueues.Add(key))
            StartCoroutine(ProcessQueue(key));
    }

    private IEnumerator ProcessQueue(Vector2Int key)
    {
        while (popupQueues[key].Count > 0)
        {
            var (text, pos, dur) = popupQueues[key].Dequeue();
            CreateTextPopup(text, pos, dur);
            yield return new WaitForSecondsRealtime(1.35f);
        }

        popupQueues.Remove(key);
        runningQueues.Remove(key);
    }

    public TextPopup CreateUITextPopup(string text, Vector3 position)
    {
        var popup = Instantiate(textPopupPrefab, canvas.transform).GetComponent<TextPopup>();

        RectTransform rect = popup.GetComponent<RectTransform>();
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-25f, 25f), 0, 0);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            position + randomOffset,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPos
        );

        rect.anchoredPosition = localPos;
        popup.Init(text);
        return popup;
    }
    
    public TextPopup CreateTextPopup(string text, Vector3 position, float durationIncrease = 0)
    {
        var popup = Instantiate(textPopupPrefab, canvas.transform).GetComponent<TextPopup>();
        activePopups.Add(popup);
        popup.OnDestroy += () => activePopups.Remove(popup);

        var rect = (RectTransform)popup.transform;

        Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        Vector2 screenPos = cam.WorldToScreenPoint(position);
        screenPos += new Vector2(UnityEngine.Random.Range(-25f, 25f), 0f);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            screenPos,
            cam,
            out Vector2 localPos
        );

        rect.anchoredPosition = localPos;
        popup.Init(text, durationIncrease);
        return popup;
    }

}
