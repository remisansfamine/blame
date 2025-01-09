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

        Vector2Int currPosition = Vector2Int.zero;
        for (currPosition.x = cellPosition.x - neighborhoodRange; currPosition.x < cellPosition.x + neighborhoodRange; currPosition.x++)
            for (currPosition.y = cellPosition.y - neighborhoodRange; currPosition.y < cellPosition.y + neighborhoodRange; currPosition.y++)
                if (virtualCells.TryGetValue(currPosition, out VirtualCellData foundNeighbor))
                    newVirtual.AddNeighbor(foundNeighbor);
        
        return newVirtual;
    }
}
