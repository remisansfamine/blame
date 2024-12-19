using UnityEngine;

public class Cell
{
    private Vector2Int position;

    public Vector2Int Position => position;

    public Cell(Vector2Int position)
    {
        this.position = position; 
    }
}
