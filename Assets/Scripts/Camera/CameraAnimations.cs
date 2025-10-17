using System;
using CodeMonkey.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraAnimations : MonoBehaviour
{
    private float originalSize;
    private Camera camera;
    [FormerlySerializedAs("duration")] [SerializeField] private float zoomDuration = 0.3f;
    [SerializeField] private Ease easeType = Ease.OutCubic;
    private float originalX;
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        originalSize = camera.orthographicSize;
        originalX = camera.transform.localPosition.x;
        BuildingUI.Instance.OnEnterBuildMode += () => ZoomOut(16);
        BuildingUI.Instance.OnExitBuildMode += ZoomBack;
        EnemyHealth.OnDeathStatic += (bool final) =>
        {
            float duration = final ? 1f : 0.1f;
            float strength = final ? 0.25f : 0.2f;
            if (final)
            {
                FunctionTimer.Create(() =>
                {
                    CameraShake.Shake(Camera.main.transform, duration, strength);
                }, 0.2f);
            }
                
        };
    }

    private void ZoomOut(float newSize)
    {
        camera.DOOrthoSize(newSize, zoomDuration).SetEase(easeType);
        camera.transform.DOMoveX(-5.3f, zoomDuration).SetEase(easeType);
    }

    private void ZoomBack()
    {
        camera.DOOrthoSize(originalSize, zoomDuration).SetEase(easeType);
        camera.transform.DOMoveX(originalX, zoomDuration).SetEase(easeType);
    }
    
}
