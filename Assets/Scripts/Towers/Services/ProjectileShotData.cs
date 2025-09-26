using UnityEngine;

public class ProjectileShotData
{
    public Color projectileColor = Color.white;
    public int plusDamage;
    public int maxEnemiesPierced;
    public bool homing;
    public float speedIncrease = 1;
    private float releaseTime;
    public float AirTime => Time.time - releaseTime;
    public ProjectileShotData(float releaseTime)
    {
        this.releaseTime = releaseTime;
    }
}