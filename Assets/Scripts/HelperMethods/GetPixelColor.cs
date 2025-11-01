using UnityEngine;
using UnityEngine.UI;

public static class GetPixelColor
{
    public static Color SampleColorAtLocalPoint(Image image, Vector2 localPoint)
    {
        if (image == null || image.sprite == null)
        {
            Debug.LogWarning("Image or sprite is null");
            return Color.clear;
        }

        Sprite sprite = image.sprite;
        Texture2D texture = sprite.texture;

        if (!texture.isReadable)
        {
            Debug.LogError("Texture must have Read/Write enabled in import settings!");
            return Color.clear;
        }

        RectTransform rectTransform = image.rectTransform;
        Rect rect = sprite.textureRect;
        
        // Calculate pivot offset
        float pivotX = sprite.pivot.x / sprite.rect.width;
        float pivotY = sprite.pivot.y / sprite.rect.height;
        
        // Convert local point to texture coordinates
        float normalizedX = (localPoint.x / rectTransform.rect.width) + pivotX;
        float normalizedY = (localPoint.y / rectTransform.rect.height) + pivotY;
        
        // Convert to texture pixel coordinates
        int pixelX = Mathf.Clamp((int)(normalizedX * rect.width + rect.x), 0, texture.width - 1);
        int pixelY = Mathf.Clamp((int)(normalizedY * rect.height + rect.y), 0, texture.height - 1);
        
        return texture.GetPixel(pixelX, pixelY);
    }
    public static Color SampleColorAtAngle(Image image, float angleDegrees, float radiusNormalized = 0.4f)
    {
        if (image == null || image.rectTransform == null)
        {
            Debug.LogWarning("Image or RectTransform is null");
            return Color.clear;
        }
        
        // Convert angle to radians
        float radians = angleDegrees * Mathf.Deg2Rad;
        
        // Calculate radius in local space
        float radius = image.rectTransform.rect.width * radiusNormalized;
        
        // Calculate local point
        Vector2 localPoint = new Vector2(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius
        );
        
        return SampleColorAtLocalPoint(image, localPoint);
    }
    
    /// <summary>
    /// Samples color where one RectTransform points to another
    /// </summary>
    /// <param name="image">The UI Image to sample from</param>
    /// <param name="fromTransform">The transform pointing at the image (e.g., a marker)</param>
    /// <param name="radiusNormalized">Optional: sample at specific radius instead of exact point</param>
    /// <returns>The color at that direction</returns>
    public static Color SampleColorAtTransform(Image image, RectTransform fromTransform, float radiusNormalized = -1f)
    {
        if (image == null || fromTransform == null)
        {
            Debug.LogWarning("Image or fromTransform is null");
            return Color.clear;
        }
        
        RectTransform imageRect = image.rectTransform;
        
        // Calculate direction from image center to marker
        Vector2 direction = (fromTransform.position - imageRect.position).normalized;
        
        // Calculate angle considering image rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = (angle - imageRect.eulerAngles.z + 360) % 360;
        
        // Use specified radius or calculate from actual distance
        float radius = radiusNormalized;
        if (radiusNormalized < 0)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imageRect,
                fromTransform.position,
                null,
                out localPoint
            );
            radius = localPoint.magnitude / imageRect.rect.width;
        }
        
        return SampleColorAtAngle(image, angle, radius);
    }

}
