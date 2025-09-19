using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamageMarker : MonoBehaviour
{
    private TextMeshPro text;
    public bool AddPunchEffect { get; private set; }
    public Vector2 RandomPosition { get; private set; }
    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        AddPunchEffect = false;
    }

    public void Init(int damage, Color[] colors, float sizeMult, bool punchEffect, Vector2 randomPosition)
    { 
        
        text.text = damage.ToString();
        AddPunchEffect = punchEffect;
        text.rectTransform.localScale = Vector3.one * sizeMult;
        RandomPosition = randomPosition;
        Color topLeft, topRight, bottomLeft, bottomRight;
        switch (colors.Length)
        {
            case 1:
                topLeft = topRight = bottomLeft = bottomRight = colors[0];
                break;
            case 2:
                topLeft = topRight = colors[0];
                bottomLeft = bottomRight = colors[1];
                break;
            case 3:
                topLeft = colors[0];
                topRight = colors[1];
                bottomLeft = bottomRight = colors[2];
                break;
            case 4:
                topLeft = colors[0];
                topRight = colors[1];
                bottomLeft = colors[2];
                bottomRight = colors[3];
                break;
            default:
                topLeft = topRight = bottomLeft = bottomRight = Color.white;
                break;
        }
        
        
        var gradient = new VertexGradient(topLeft, topRight, bottomLeft, bottomRight);
        text.enableVertexGradient = true;
        text.colorGradient = gradient;
    }
}