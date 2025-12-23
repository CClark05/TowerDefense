using System;
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
    public ProjectileShotData(float releaseTime)
    {
        this.releaseTime = releaseTime;
    }

    public event Action OnShotDestroyed;


    public void ShotDestroyed()
    {
        if (OnShotDestroyed != null)
        {
            OnShotDestroyed.Invoke();
            return;
        }
        UnityEngine.Object.Destroy(projectile.gameObject);
    }
}