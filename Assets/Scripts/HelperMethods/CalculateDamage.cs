using System;

public static class CalculateDamage
{
    public static int MultIncrease(float mult, int original, int playCount)
    {
        return (int)Math.Floor(original * (mult * playCount));
    }
    public static float MultIncrease(float mult, float original, int playCount)
    {
        return (original * (mult * playCount));
    }
}
