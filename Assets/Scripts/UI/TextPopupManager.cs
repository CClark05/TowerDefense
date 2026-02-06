using System;
using TMPro;
using UnityEngine;

public class TextPopupManager : Singleton<TextPopupManager>
{
    [SerializeField] private GameObject textPopupPrefab;
    private Canvas canvas;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        SkillInstance.OnDisposeSkill += (instance,position) =>
        {
            string text = $"<b><size=125%>{instance.Data.name}</size></b> Destroyed";
            var popup = CreateTextPopup(text, position + new Vector2(6,0), 0.5f);
            popup.GetComponent<TextMeshProUGUI>().color = ColorPicker.red;
        };
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
        var rect = (RectTransform)popup.transform;
        
        Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        Vector2 screenPos = cam.WorldToScreenPoint(position);
        
        screenPos += new Vector2(UnityEngine.Random.Range(-25f, 25f), 0f);
        
        RectTransform canvasRect = (RectTransform)canvas.transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            cam, 
            out Vector2 localPos
        );
        
        rect.anchoredPosition = localPos;

        popup.Init(text, durationIncrease);
        return popup;
    }
}
