using System;
using System.Collections.Generic;
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
        baseSpeed = GetComponent<EnemyDataHolder>().Data.speed;
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
}
