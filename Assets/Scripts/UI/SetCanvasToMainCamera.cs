using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SetCanvasToMainCamera : MonoBehaviour
{
    [SerializeField] private string sortingLayerName = "UI";
    [SerializeField] private int sortingOrder;
    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = sortingLayerName;
        canvas.sortingOrder = sortingOrder;
    }
}