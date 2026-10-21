using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileShotData
{
    public Color projectileColor = Color.white;
    public Sprite projectileSprite;
    public int plusDamage;
    public float plusSizePercent;

    public int maxEnemiesPierced;

    //public bool homing;
    public float speedIncrease = 1;
    private float releaseTime;
    public float AirTime => Time.time - releaseTime;
    public Vector2 originalDirection;
    public readonly List<Vector2> directionOverrides;
    public int homingProjectiles;
    public int projectiles;
    public float? lifetimeDuration;
    
    public ProjectileShotData(float releaseTime)
    {
        this.releaseTime = releaseTime;
        directionOverrides = new();
    }

    private int currentProjectileAmount;
    public void RegisterProjectile(Projectile projectile)
    {
        currentProjectileAmount++;
        if (currentProjectileAmount <= homingProjectiles)
        {
            projectile.SetHoming();
            Debug.Log("homing");
        }
        
    }

    public event Action<Projectile> OnShotDestroyed;

    public void ShotDestroyed(Projectile projectile, out bool eventInvoked)
    {
        if (OnShotDestroyed != null)
        {
            OnShotDestroyed.Invoke(projectile);
            Debug.Log("ShotDestroyed event invoked");
            eventInvoked = true;
            return;
        }

        eventInvoked = false;
        //UnityEngine.Object.Destroy(projectile.gameObject);
    }

    public void SetLifetime(float duration)
    {
        
    }
}