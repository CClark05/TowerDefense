using UnityEngine;

public class UpgradeSlotAnimation : MonoBehaviour
{
    private ColorAnimation colorAnimation;

    private void Awake()
    {
        colorAnimation = GetComponent<ColorAnimation>();
        colorAnimation.enabled = false;
    }
    public void ToggleAnimation(bool enabled)
    {
        colorAnimation.enabled = enabled;
    }
}