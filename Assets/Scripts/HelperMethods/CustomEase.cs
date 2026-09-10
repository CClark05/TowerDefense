public static class CustomEase
{
    public static  float EaseOutBounceCustom(float time, float duration, float overshootOrAmplitude, float period)
    {
        float x = time / duration;

        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        float result;

        if (x < 1f / d1)
            result = n1 * x * x;
        else if (x < 2f / d1)
            result = n1 * (x -= 1.5f / d1) * x + 0.75f;
        else if (x < 2.5f / d1)
            result = n1 * (x -= 2.25f / d1) * x + 0.9375f;
        else
            result = n1 * (x -= 2.625f / d1) * x + 0.984375f;
        result = 1f - (1f - result) * 0.8f;
        return result;
    }
}
