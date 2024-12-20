using System.Collections.Generic;
using UnityEngine;

public class CellsData
{
    public enum CellState
    {
        EMPTY,      //  Cell is empty, but in at least the virtual range
        VIRTUAL,    //  Cell is supposed to be filled but not close enough to be filled / rendered
        LOADED      //  Cell is filled and in the render range 
    }

    private HashSet<Vector2Int> emptyCells = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, VirtualCellData>  virtualCells = new Dictionary<Vector2Int, VirtualCellData>();
    private Dictionary<Vector2Int, CellStructure> loadedCells = new Dictionary<Vector2Int, CellStructure>();

    public HashSet<Vector2Int> EmptyCells => emptyCells;
    public Dictionary<Vector2Int, VirtualCellData> VirtualCells => virtualCells;
    public Dictionary<Vector2Int, CellStructure> LoadedCells => loadedCells;


    public bool IsCellOfState(Vector2Int cellCoordinate, CellState State)
    {
        switch (State)
        {
            case CellState.EMPTY:
                return emptyCells.Contains(cellCoordinate);

            case CellState.VIRTUAL:
                return virtualCells.ContainsKey(cellCoordinate);

            case CellState.LOADED:
                return loadedCells.ContainsKey(cellCoordinate);
            
            default: return false;
        }
    }

    public void ClearAll()
    {
        emptyCells.Clear();
        virtualCells.Clear();

        foreach (var cell in loadedCells)
            Object.Destroy(cell.Value.gameObject);

        loadedCells.Clear();
    }

    public void ClearAt(Vector2Int position)
    {
        if (emptyCells.Remove(position))
            return;

        if (virtualCells.Remove(position))
        {
            if (loadedCells.TryGetValue(position, out var cell))
            {
                Object.Destroy(cell.gameObject);
                loadedCells.Remove(position);
            }
        }
    }
}
