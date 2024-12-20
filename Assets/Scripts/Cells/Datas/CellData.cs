using UnityEngine;

public class CellData
{
    private Vector2Int position;

    public Vector2Int Position => position;

    public CellData(Vector2Int position)
    {
        this.position = position; 
    }
}
