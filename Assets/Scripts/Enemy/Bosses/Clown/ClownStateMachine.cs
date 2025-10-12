using System;

public class ClownStateMachine : BossStateMachine
{
    private void Start()
    {
        VignetteController.Instance.FadeTo(0.44f, 0.7f, 1f);
    }

    private void OnDestroy()
    {
        VignetteController.Instance.FadeTo(0f, 0f, 1f);
    }
}
