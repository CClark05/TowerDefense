using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class BirdMovement : MonoBehaviour, IPathPredictor, IMovementListener, IMovementOverride
{
    private float baseSpeed;
    private float speedMult = 1;
    public event Action OnReachedEnd;
    public float Progress { get; private set; }
    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2 direction;
    private List<Vector2> path = new();
    private float startingDistance;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        var random = UnityEngine.Random.Range(2.5f, -2.5f);
        transform.position = new Vector2(transform.position.x, transform.position.y + random);
        baseSpeed = GetComponent<EnemyDataHolder>().Data.speed + 1 * GetComponent<EnemyDataHolder>().SpeedIncrease;
        Debug.Log(GetComponent<EnemyDataHolder>().Data.name + "Speed " + baseSpeed);
        path = AStarPathfinding.Instance.GetPath();
        direction = (path[^1] - (Vector2)transform.position).normalized;
        velocity = direction * baseSpeed; 
        startingDistance = Vector2.Distance(path[^1], transform.position);
    }

    private void Update()
    {
        Progress = 1 - Mathf.Clamp01((path[^1] - (Vector2)transform.position).magnitude / startingDistance);
        if (Vector2.Distance(transform.position, path[^1]) < 0.1f)
        {
            OnReachedEnd?.Invoke();
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = velocity * speedMult;
    }

    public bool TryPosVelAt(float t, out Vector2 pos, out Vector2 vel)
    {
        Vector2 direction = path[^1] - (Vector2)transform.position;
        Vector2 unitDirection = (direction) / direction.magnitude; 
        pos = (Vector2)transform.position + unitDirection * (baseSpeed * t);
        vel = unitDirection * baseSpeed;
        return true;
    }


    public void SetSpeed(float speedMult, float duration = 0)
    {
        this.speedMult = speedMult;
    }

    public void ResetSpeed(float duration = 0)
    {
        speedMult = 1;
    }

    public void AddSpeed(float percentIncrease)
    {
        speedMult += percentIncrease;
    }

    private Tween moveTween;
    public void MoveTo(Vector2 position, float duration)
    {
        if (moveTween != null) return;
        velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        moveTween = rb.DOMove(position, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() =>
            {
                Vector2 target = path[^1];
                direction = (target - (Vector2)transform.position).normalized;
                velocity = direction * baseSpeed;
                moveTween = null;
            });
    }
}
