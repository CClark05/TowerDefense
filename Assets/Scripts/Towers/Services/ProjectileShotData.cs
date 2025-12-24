using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShotData
{
    public Color projectileColor = Color.white;
    public Sprite projectileSprite;
    public int plusDamage;
    public int maxEnemiesPierced;
    public bool homing;
    public float speedIncrease = 1;
    private float releaseTime;
    public float AirTime => Time.time - releaseTime;
    public Projectile projectile;
    public Vector2 originalDirection;
    public readonly List<Vector2> directionOverrides;
    public ProjectileShotData(float releaseTime)
    {
        this.releaseTime = releaseTime;
        directionOverrides = new();
    }

    public event Action<Projectile> OnShotDestroyed;
    
    public void ShotDestroyed(Projectile projectile)
    {
        if (OnShotDestroyed != null)
        {
            OnShotDestroyed.Invoke(projectile);
            return;
        }
        UnityEngine.Object.Destroy(projectile.gameObject);
    }
}