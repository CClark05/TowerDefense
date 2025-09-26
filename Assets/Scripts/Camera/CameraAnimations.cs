using System;
using DG.Tweening;
using UnityEngine;

public class CameraAnimations : MonoBehaviour
{
    private float originalSize;
    private Camera camera;
    [SerializeField] private float duration = 0.3f;
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
        EnemyHealth.OnFinalEnemyDeath += () => CameraShake.ShakeDefault();
    }

    private void ZoomOut(float newSize)
    {
        camera.DOOrthoSize(newSize, duration).SetEase(easeType);
        camera.transform.DOMoveX(-5.3f, duration).SetEase(easeType);
    }

    private void ZoomBack()
    {
        camera.DOOrthoSize(originalSize, duration).SetEase(easeType);
        camera.transform.DOMoveX(originalX, duration).SetEase(easeType);
    }
    
}
