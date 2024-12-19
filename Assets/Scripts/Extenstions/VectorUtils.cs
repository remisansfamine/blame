using Unity.VisualScripting;
using UnityEngine;

public static class VectorUtils
{
    public static int SqrdDistance(this Vector2Int self, Vector2Int other)
    {
        Vector2Int difference = self - other;
        return difference.x * difference.x + difference.y * difference.y;
    }
    
}
