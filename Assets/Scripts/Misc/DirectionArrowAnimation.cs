using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DirectionArrowAnimation : MonoBehaviour
{
    private Vector2 originalPos;
    private SpriteRenderer sr;
    private Vector2 direction;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var fullPath = AStarPathfinding.Instance.GetPath();
        Vector3[] path = fullPath
            .Take(fullPath.Count / 2)
            .Select(p => (Vector3)p)
            .ToArray();
        
        float duration = path.Length / 10f;
        
        direction = (path[1] - path[0]).normalized;
        ApplyRotation();
        Tween move = transform.DOPath(path, duration)
            .SetEase(Ease.Linear)
            .OnWaypointChange(i =>
            {
                if (i + 1 < path.Length)
                {
                    direction = (path[i + 1] - path[i]).normalized;
                    ApplyRotation();
                }
            });

        Sequence seq = DOTween.Sequence();
        seq.Append(move);
        seq.Insert(duration * 0.5f, sr.DOFade(0, duration * 0.5f).SetEase(Ease.InCubic));
        seq.SetLoops(-1, LoopType.Restart);
        seq.SetLink(gameObject);
    }

    private void ApplyRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180);
    }
    //down is 90
    //up is -90
    
}
