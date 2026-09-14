using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public event Action<TowerDataHolder> OnHit;
    private Transform target;
    private Rigidbody2D rb;
    private float speed = 5f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<TowerDataHolder>() != null)
        {
            var towerDataHolder = other.GetComponent<TowerDataHolder>();
            OnHit?.Invoke(towerDataHolder);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            Vector2 newPosition = rb.position + direction * (speed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }
}
