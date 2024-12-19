using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class VirtualCell : Cell
{
    public HashSet<VirtualCell> neighbors = new HashSet<VirtualCell>();

    public VirtualCell(Vector2Int position) : base(position)
    { }

    public void AddNeighbors(HashSet<VirtualCell> newNeighbors)
    {
        neighbors.AddRange(newNeighbors);

        foreach (VirtualCell neighbor in neighbors)
            neighbor.neighbors.Add(this);
    }
}
