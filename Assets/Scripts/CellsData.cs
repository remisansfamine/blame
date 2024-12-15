using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;



public class CellsData
{
    public enum CellState
    {
        EMPTY,      //  Cell is empty, but in at least the virtual range
        VIRTUAL,    //  Cell is supposed to be filled but not close enough to be filled / rendered
        RENDERED      //  Cell is filled and in the render range 
    }

    private HashSet<Vector2Int> emptyCells   = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> virtualCells = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> renderedCells  = new HashSet<Vector2Int>();

    public bool IsCellOfState(Vector2Int cellCoordinate, CellState State)
    {
        switch (State)
        {
            case CellState.EMPTY:
                return emptyCells.Contains(cellCoordinate);

            case CellState.VIRTUAL:
                return virtualCells.Contains(cellCoordinate);

            case CellState.RENDERED:
                return renderedCells.Contains(cellCoordinate);
            
            default: return false;
        }
    }
    
    public void RegisterCellByState(Vector2Int cellCoordinate, CellState State)
    {
        switch (State)
        {
            case CellState.EMPTY:
                emptyCells.Add(cellCoordinate);
                break;

            case CellState.VIRTUAL:
                virtualCells.Add(cellCoordinate);
                break;

            case CellState.RENDERED:
                renderedCells.Add(cellCoordinate);
                break;
        }
    }

    public void UnregisterCellByState(Vector2Int cellCoordinate, CellState State)
    {
        switch (State)
        {
            case CellState.EMPTY:
                emptyCells.Remove(cellCoordinate);
                break;

            case CellState.VIRTUAL:
                virtualCells.Remove(cellCoordinate);
                break;

            case CellState.RENDERED:
                renderedCells.Remove(cellCoordinate);
                break;
        }
    }
}
