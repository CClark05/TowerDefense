using System;
using UnityEngine;

public class ResourceVisual : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    private Material originalMaterial;
    private SpriteRenderer sr;
    private Resource resource;
    private void Awake()
    {
        originalMaterial = GetComponent<SpriteRenderer>().material;
        sr = GetComponent<SpriteRenderer>();
        resource = GetComponent<Resource>();
    }

    private void Start()
    {
        resource.OnHoverEvent += OnHover;
        resource.OnHoverLeaveEvent += OnLeaveHover;
    }

    private void OnLeaveHover() => sr.material = originalMaterial;

    private void OnHover() => sr.material = outlineMaterial;

    private void OnDestroy()
    {
        resource.OnHoverEvent -= OnHover;
        resource.OnHoverLeaveEvent -= OnLeaveHover;
    }
}
