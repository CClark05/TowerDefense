using UnityEngine;

public interface ITowerAimOverride
{
    bool CanShoot(GameObject target);
    Vector2 GetShotDirection(Vector2 origin, GameObject target);
}