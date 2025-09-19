using UnityEngine;

public class ClampWorldSpaceUI : MonoBehaviour
{
    [SerializeField] private float screenPadding = 20f;

    private Camera cam;
    private RectTransform rectTransform;

    void Start()
    {
        cam = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        ClampUIOnScreen();
    }

    private void ClampUIOnScreen()
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        Vector3 maxOffset = Vector3.zero;

        foreach (var corner in worldCorners)
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(corner);

            float xOffset = 0f;
            float yOffset = 0f;

            if (screenPoint.x < 0)
                xOffset = 0 - screenPoint.x;
            else if (screenPoint.x > Screen.width)
                xOffset = Screen.width - screenPoint.x;

            if (screenPoint.y < 0)
                yOffset = 0 - screenPoint.y;
            else if (screenPoint.y > Screen.height)
                yOffset = Screen.height - screenPoint.y;

            // Convert screen offset to world space
            Vector3 worldOffset = cam.ScreenToWorldPoint(screenPoint + new Vector3(xOffset, yOffset, 0))
                                  - cam.ScreenToWorldPoint(screenPoint);

            // Keep the biggest required correction in either axis
            if (Mathf.Abs(worldOffset.x) > Mathf.Abs(maxOffset.x))
                maxOffset.x = worldOffset.x;
            if (Mathf.Abs(worldOffset.y) > Mathf.Abs(maxOffset.y))
                maxOffset.y = worldOffset.y;
        }

        transform.position += maxOffset;
    }
}