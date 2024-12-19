using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CellsData
{
    public enum CellState
    {
        EMPTY,      //  Cell is empty, but in at least the virtual range
        VIRTUAL,    //  Cell is supposed to be filled but not close enough to be filled / rendered
        RENDERED      //  Cell is filled and in the render range 
    }

    private HashSet<Vector2Int> emptyCells = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, VirtualCell>  virtualCells = new Dictionary<Vector2Int, VirtualCell>();
    private Dictionary<Vector2Int, GameObject> renderedCells = new Dictionary<Vector2Int, GameObject>();

    public HashSet<Vector2Int> EmptyCells => emptyCells;
    public Dictionary<Vector2Int, VirtualCell> VirtualCells => virtualCells;
    public Dictionary<Vector2Int, GameObject> RenderedCells => renderedCells;


    public bool IsCellOfState(Vector2Int cellCoordinate, CellState State)
    {
        switch (State)
        {
            case CellState.EMPTY:
                return emptyCells.Contains(cellCoordinate);

            case CellState.VIRTUAL:
                return virtualCells.ContainsKey(cellCoordinate);

            case CellState.RENDERED:
                return renderedCells.ContainsKey(cellCoordinate);
            
            default: return false;
        }
    }

    public void ClearAll()
    {
        emptyCells.Clear();
        virtualCells.Clear();

        foreach (var cell in renderedCells)
            Object.Destroy(cell.Value);

        renderedCells.Clear();
    }

    public void ClearAt(Vector2Int position)
    {
        if (emptyCells.Remove(position))
            return;

        if (virtualCells.Remove(position))
        {
            if (renderedCells.TryGetValue(position, out var cell))
            {
                Object.Destroy(cell);
                renderedCells.Remove(position);
            }
        }
    }
}
