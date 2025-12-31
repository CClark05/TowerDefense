using UnityEngine;

public interface IMovementOverride
{
    void SetSpeed(float speedMult, float duration = 0);
    void ResetSpeed(float duration = 0);
    void AddSpeed(float percentIncrease);
    void MoveTo(Vector2 position, float duration);
}