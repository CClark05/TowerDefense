using UnityEngine;

public interface IMovementOverride
{
    void SetSpeed(float speedMult, float duration = 0);
    void ResetSpeed(float duration = 0);
    void AddSpeed(float percentIncrease);
    void TeleportTo(Vector2 position);
    void MoveTo(Vector2 position, float duration);
}