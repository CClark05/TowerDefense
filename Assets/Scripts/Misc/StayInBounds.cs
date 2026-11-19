using UnityEngine;

public class StayInBounds : MonoBehaviour
{
    private Camera cam;
    private SpriteRenderer sr;

    private void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if (!IsOffScreen()) return;
        Bounds b = sr.bounds;
        Rect r = GetScreenWorldRect();
        Vector3 shift = Vector3.zero;

        if (b.min.x < r.xMin) shift.x = r.xMin - b.min.x;
        else if (b.max.x > r.xMax) shift.x = r.xMax - b.max.x;

        if (b.min.y < r.yMin) shift.y = r.yMin - b.min.y;
        else if (b.max.y > r.yMax) shift.y = r.yMax - b.max.y;

        transform.position += shift;
    }

    private Rect GetScreenWorldRect()
    {
        float h = cam.orthographicSize * 2f;
        float w = h * cam.aspect;
        Vector2 c = cam.transform.position;
        return new Rect(c.x - w / 2f, c.y - h / 2f, w, h);
    }

    private bool IsOffScreen()
    {
        Bounds b = sr.bounds;
        Rect r = GetScreenWorldRect();
        return b.min.x < r.xMin || b.max.x > r.xMax || b.min.y < r.yMin || b.max.y > r.yMax;
    }
}