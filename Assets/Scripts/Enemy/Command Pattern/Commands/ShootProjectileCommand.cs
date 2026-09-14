using System;
using System.Collections;
using UnityEngine;

public class ShootProjectileCommand : EnemyCommand
{
    private GameObject projectilePrefab;
    private Transform target;
    private float speed;
    private Action<TowerDataHolder> OnHit;
    public ShootProjectileCommand(GameObject projectilePrefab, Transform target, float speed, Action<TowerDataHolder> OnHit)
    {
        this.projectilePrefab = projectilePrefab;
        this.target = target;
        this.speed = speed;
        this.OnHit = OnHit;
    }
    public override IEnumerator Execute(IAgent agent)
    {
        var projectile = GameObject.Instantiate(projectilePrefab, agent.Transform.position, Quaternion.identity);
        if (projectile.GetComponent<EnemyProjectile>() == null)
        {
            Debug.LogError("No enemy projectile component");
            yield return null;
        }
        var enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        enemyProjectile.Init(target, speed);
        enemyProjectile.OnHit += tower =>
        {
            OnHit?.Invoke(tower);
        };
    }
}