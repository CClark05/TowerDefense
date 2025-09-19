using System;
using UnityEngine;

public class ProjectileVisual : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }
}
