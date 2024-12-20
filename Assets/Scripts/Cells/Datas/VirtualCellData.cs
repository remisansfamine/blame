using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class VirtualCellData : CellData
{
    public HashSet<VirtualCellData> neighbors = new HashSet<VirtualCellData>();

    public VirtualCellData(Vector2Int position) : base(position)
    { }

    public void AddNeighbors(HashSet<VirtualCellData> newNeighbors)
    {
        neighbors.AddRange(newNeighbors);

        foreach (VirtualCellData neighbor in neighbors)
            neighbor.neighbors.Add(this);
    }
}
