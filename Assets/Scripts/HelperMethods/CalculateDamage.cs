using System;

public static class CalculateDamage
{
    public static int MultIncrease(float mult, int original)
    {
        return (int)Math.Floor(original * (1 + mult));
    }
}
