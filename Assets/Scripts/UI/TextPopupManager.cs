using System;
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

    public TextPopup CreateTextPopup(string text, Vector3 position)
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            CreateTextPopup("NO ROOM", Input.mousePosition);
    }
}
