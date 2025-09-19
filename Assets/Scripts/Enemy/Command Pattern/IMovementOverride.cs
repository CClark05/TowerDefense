public interface IMovementOverride
{
    void SetSpeed(float speedMult, float duration = 0);
    void ResetSpeed(float duration = 0);
}