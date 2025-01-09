using Unity.VisualScripting;
using UnityEngine;

public static class VectorUtils
{
    public static Vector3 AsFlatVector3(this Vector2 self)
    {
        return new Vector3(self.x, 0f, self.y);
    }

    public static int SqrdDistance(this Vector2Int self, Vector2Int other)
    {
        Vector2Int difference = self - other;
        return difference.x * difference.x + difference.y * difference.y;
    }
    
}
