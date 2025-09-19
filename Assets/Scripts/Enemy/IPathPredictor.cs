using UnityEngine;

public interface IPathPredictor
{
    bool TryPosVelAt(float t, out Vector2 pos, out Vector2 vel);
}