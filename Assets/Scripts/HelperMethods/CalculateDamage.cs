using System;

public static class CalculateDamage
{
    public static int MultIncrease(float mult, int original)
    {
        return (int)Math.Round(original * (1 + mult), MidpointRounding.AwayFromZero);
    }
}
