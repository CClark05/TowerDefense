using UnityEngine;

public static class SpriteColorUtils
{
    /// <summary>
    /// Returns the average color of visible pixels in a sprite (ignores alpha < alphaThreshold).
    /// Works with atlased sprites via textureRect.
    /// </summary>
    public static Color AverageColor(Sprite sprite, float alphaThreshold = 0.1f)
    {
        if (sprite == null || sprite.texture == null)
            return Color.white;

        var tex = sprite.texture;
        var r = sprite.textureRect; // rect in texture space (pixels)
        int x = Mathf.RoundToInt(r.x);
        int y = Mathf.RoundToInt(r.y);
        int w = Mathf.RoundToInt(r.width);
        int h = Mathf.RoundToInt(r.height);

        Color[] pixels;
        try
        {
            pixels = tex.GetPixels(x, y, w, h); // Color[] 0..1
        }
        catch
        {
            Debug.LogError($"AverageColor: texture not readable for '{sprite.name}'.");
            return Color.white;
        }

        double rs = 0, gs = 0, bs = 0, as_ = 0;
        int count = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            var p = pixels[i];           // p.r,g,b,a are already 0..1
            float a = p.a;               // <-- fixed
            if (a >= alphaThreshold)
            {
                rs += p.r;
                gs += p.g;
                bs += p.b;
                as_ += p.a;
                count++;
            }
        }

        if (count == 0) return Color.white;

        return new Color(
            (float)(rs / count),
            (float)(gs / count),
            (float)(bs / count),
            (float)(as_ / count)
        );
    }
}