using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShotData
{
    public Color projectileColor = Color.white;
    public Sprite projectileSprite;
    public int plusDamage;
    public int maxEnemiesPierced;
    //public bool homing;
    public float speedIncrease = 1;
    private float releaseTime;
    public float AirTime => Time.time - releaseTime;
    public Vector2 originalDirection;
    public readonly List<Vector2> directionOverrides;
    public int homingProjectiles;
    private HashSet<Projectile> projectiles = new();
    public ProjectileShotData(float releaseTime)
    {
        this.releaseTime = releaseTime;
        directionOverrides = new();
    }

    public void RegisterProjectile(Projectile projectile)
    {
        projectiles.Add(projectile);
        if (projectiles.Count <= homingProjectiles)
        {
            projectile.SetHoming();
            Debug.Log("homing");
        }
    }
    public event Action<Projectile> OnShotDestroyed;
    
    public void ShotDestroyed(Projectile projectile)
    {
        if (OnShotDestroyed != null)
        {
            OnShotDestroyed.Invoke(projectile);
            Debug.Log("ShotDestroyed event invoked");
            return;
        }
        UnityEngine.Object.Destroy(projectile.gameObject);
    }
}