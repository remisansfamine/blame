using System;
using System.Collections.Generic;
using UnityEngine;

public static class RectExtensions
{
    public static Vector2 ClampPosition(this Rect self, Vector2 position) => new Vector2(
            Mathf.Clamp(position.x, self.xMin, self.xMax), 
            Mathf.Clamp(position.y, self.yMin, self.yMax)
    );

    public static Vector2 PointOnSurface(this Rect self, Vector2 direction)
    {
        float absDirX = Mathf.Abs(direction.x);
        float absDirY = Mathf.Abs(direction.y);

        float scaleX = (absDirX > 0) ? (self.width * 0.5f) / absDirX : float.MaxValue;
        float scaleY = (absDirY > 0) ? (self.height * 0.5f) / absDirY : float.MaxValue;

        return self.center + direction * Mathf.Min(scaleX, scaleY);
    }


    public static Rect[] Subtract(this Rect self, Rect other)
    {
        // If the two rectangles don't overlap, return the original rectangle
        if (!self.Overlaps(other))
            return new[] { self };

        List<Rect> result = new List<Rect>();

        // Determine the horizontal and vertical regions that remain after subtraction
        // Split the rectangle based on the overlap area

        // Top region (above the overlapping part)
        if (other.yMax < self.yMax)
        {
            result.Add(new Rect(self.xMin, other.yMax, self.width, self.yMax - other.yMax));
        }

        // Bottom region (below the overlapping part)
        if (other.yMin > self.yMin)
        {
            result.Add(new Rect(self.xMin, self.yMin, self.width, other.yMin - self.yMin));
        }

        // Left region (left of the overlapping part)
        if (other.xMin > self.xMin)
        {
            result.Add(new Rect(self.xMin, Math.Max(self.yMin, other.yMin), other.xMin - self.xMin, Math.Min(self.yMax, other.yMax) - Math.Max(self.yMin, other.yMin)));
        }

        // Right region (right of the overlapping part)
        if (other.xMax < self.xMax)
        {
            result.Add(new Rect(other.xMax, Math.Max(self.yMin, other.yMin), self.xMax - other.xMax, Math.Min(self.yMax, other.yMax) - Math.Max(self.yMin, other.yMin)));
        }

        return result.ToArray();
    }

    public static void DrawRect(this Rect rect, Color color, float duration = 5)
    {
        Vector3 topLeft = new Vector3(rect.xMin, 0, rect.yMax);
        Vector3 topRight = new Vector3(rect.xMax, 0, rect.yMax);
        Vector3 bottomRight = new Vector3(rect.xMax, 0, rect.yMin);
        Vector3 bottomLeft = new Vector3(rect.xMin, 0, rect.yMin);

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }
}
