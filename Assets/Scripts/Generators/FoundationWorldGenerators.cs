using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FoundationWorldGenerator", menuName = "Generators/FoundationWorldGenerator")]
public class FoundationWorldGenerators : WorldGenerator
{

    protected override VirtualCellData InstantiateVirtualCell(Vector2Int cellPosition) => new FoundationVirtualCellData(cellPosition);


    protected override VirtualCellData CreateVirtualCell(Dictionary<Vector2Int, VirtualCellData> virtualCells, Vector2Int cellPosition)
    {
        FoundationVirtualCellData newVirtual = (FoundationVirtualCellData)base.CreateVirtualCell(virtualCells, cellPosition);

        //  Neighbor logic

        HashSet<VirtualCellData> occupiedCellsInRange = virtualCells.Where(pair => pair.Key.SqrdDistance(cellPosition) <= neighborhoodRange * neighborhoodRange)
                                                                              .Select(pair => pair.Value)
                                                                              .ToHashSet();
        newVirtual.AddNeighbors(occupiedCellsInRange);
        
        return newVirtual;
    }
}
