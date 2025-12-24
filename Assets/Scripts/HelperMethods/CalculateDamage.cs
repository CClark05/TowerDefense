using System;

public static class CalculateDamage
{
    public static int MultIncrease(float mult, int original, int playCount)
    {
        return (int)Math.Floor(original * (1 + mult * playCount));
    }
}
