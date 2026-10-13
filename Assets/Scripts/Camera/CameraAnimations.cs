using System;
using CameraShake;
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
        BuildingManager.Instance.OnEnterBuildMode += () => ZoomOut(13);
        BuildingManager.Instance.OnExitBuildMode += ZoomBack;
        EnemyHealth.OnDeathStatic += (bool final) =>
        {
            if (final)
            {
                
                FunctionTimer.Create(() =>
                {
                    CameraShaker.Presets.Explosion2D(1, 3, 1);
                }, 0.1f);
                return;
            }
            CameraShaker.Presets.ShortShake2D();
        };
        EnemyManager.Instance.OnEnemyReachedEnd += _ =>
        {
            CameraShaker.Presets.Explosion2D(0.5f);
        };
    }

    private void ZoomOut(float newSize)
    {
        camera.DOOrthoSize(newSize, zoomDuration).SetEase(easeType);
    }

    private void ZoomBack()
    {
        camera.DOOrthoSize(originalSize, zoomDuration).SetEase(easeType);
    }
    
}
