using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerator", menuName = "Generators/WorldGenerator")]
public class WorldGenerator : ScriptableObject
{
    //  FIELDS
    [SerializeField] protected Vector2 offset;

    [SerializeField] protected float cellScale = 1f;

    [SerializeField] protected Grid worldGrid = new Grid();
    
    [SerializeField, Range(0f, 1f)] protected float spawnRate = 0.5f;

    [SerializeField, Min(0f)] protected float noiseResolution = 1f;

    [SerializeField, Min(0f)] protected int neighborhoodRange = 10;

    [SerializeField] protected CellStructure cellStructuresTemplate;

    //  PROPERTIES
    public float CellScale => cellScale;

    //  METHODS
    protected Vector2Int FromWorldSpaceToGridSpace(Vector3 WorldCoordinate)
    {
        return new Vector2Int(Mathf.CeilToInt(WorldCoordinate.x / CellScale), Mathf.CeilToInt(WorldCoordinate.z / CellScale)); 
    }

    protected Vector3 FromGridSpaceToWorldSpace(int x, int y)
    {
        return new Vector3(x * CellScale, 0f, y * CellScale);
    }

    protected float GetNoiseValueFromChunkSpace(float x, float y, float seed = 0)
    {
        float noiseScale = 1f / noiseResolution;
        return Mathf.PerlinNoise(seed + x * noiseScale + offset.x, seed + y * noiseScale + offset.y);
    }

    float GetNoiseValueFromWorldSpace(Vector2Int Coordinates)
    {
        return 0.0f;
    }


    public void UpdateCells(ChunkLoadComponent chunkLoader, WorldCache cellsData, Transform cellContainer)
    {
        Vector2Int loaderPosition   = FromWorldSpaceToGridSpace(chunkLoader.transform.position);
        Vector2Int oldPosition2D    = FromWorldSpaceToGridSpace(chunkLoader.OldPosition);
        Vector2Int deltaPosition    = loaderPosition - oldPosition2D;
        Vector2Int extent           = Vector2Int.one * chunkLoader.VirtualDistance;

        Rect newRect = new Rect(loaderPosition - extent, extent*2);
        
        float sqrMovement = deltaPosition.sqrMagnitude;

        //  Clearing part
        //  -------------

        //  if the distance traveled is higher than the virtual distance, clear all datas
        if (sqrMovement > chunkLoader.SquaredVirtualDistance * 4)
            Wipe(cellsData);
        else
            SweepBehind(cellsData, new Rect(oldPosition2D - extent, extent * 2), newRect);

        //  Loading part
        //  ------------

        Vector2Int currPosition = Vector2Int.zero;
        for (currPosition.x = (int)newRect.xMin; currPosition.x < (int)newRect.xMax; currPosition.x++)
        {
            for (currPosition.y = (int)newRect.yMin; currPosition.y < (int)newRect.yMax; currPosition.y++)
            {
                if (cellsData.IsCellOfState(currPosition, WorldCache.CellState.EMPTY))
                    continue;

                //  Use float to compare distances to avoid a shift
                Vector2 currPositionf = currPosition + Vector2.one * 0.5f;

                //  Calculate square distance between current coordinate and chunk loader
                float distSqr = Vector2.SqrMagnitude(currPositionf - loaderPosition);

                //int DistSqr = currPosition.SqrdDistance(loaderPosition);
                if (distSqr > (float)chunkLoader.SquaredVirtualDistance)
                    continue;

                if (cellsData.IsCellOfState(currPosition, WorldCache.CellState.LOADED))
                {
                    //  The cell is currently rendered
                    //  Should it be unloaded
                    if (distSqr > (float)chunkLoader.SquaredUnloadDistance)
                        UnloadRenderedCell(cellsData.LoadedCells, currPosition);
                    continue;
                }
                
                if (cellsData.IsCellOfState(currPosition, WorldCache.CellState.VIRTUAL))
                {
                    //  The cell is supposed to be rendered but is not rendered yet
                    //  Should we render this cell ?
                    if (distSqr <= (float)chunkLoader.SquaredRenderDistance)
                        CreateRenderedCell(cellsData.LoadedCells, cellContainer, currPosition, cellsData.VirtualCells[currPosition]);

                    continue;
                }
                //  The cell has not been registered in any state, let's check if it should be filled depending on a noise
                if (GetNoiseValueFromChunkSpace(currPosition.x, currPosition.y) >= 1f - spawnRate) 
                {
                    CreateVirtualCell(cellsData.VirtualCells, currPosition);
                    continue;
                }

                //  Mark this cell as empty as it has no state yet and was not filled by the noise
                cellsData.EmptyCells.Add(currPosition);
            }
        }
    }

    private static void Wipe(WorldCache cellsData)
    {
        cellsData.ClearAll();
    }

    
    //  Sweep all cells that were in the previous area but not in the new area
    private static void SweepBehind(WorldCache cellsData, Rect previousArea, Rect newArea, bool drawDebug = false)
    {
        Rect[] areaToClear = previousArea.Subtract(newArea);

        foreach (Rect rect in areaToClear)
        {
            if (drawDebug)
                rect.DrawRect(Color.red, 1);
            
            Vector2Int pos = Vector2Int.zero;
            for (pos.x = (int)rect.xMin; pos.x < (int)rect.xMax; pos.x++)
                for (pos.y = (int)rect.yMin; pos.y < (int)rect.yMax; pos.y++)
                    cellsData.ClearAt(pos);
        }
    }

    protected virtual void UnloadRenderedCell(Dictionary<Vector2Int, CellStructure> renderedCells, Vector2Int cellPosition)
    {
        Destroy(renderedCells[cellPosition].gameObject);
        renderedCells.Remove(cellPosition);
    }

    protected virtual void CreateRenderedCell(Dictionary<Vector2Int, CellStructure> renderedCells, Transform cellContainer, Vector2Int cellPosition, VirtualCellData cellData)
    {
        Vector3 generatedChunkPosition = FromGridSpaceToWorldSpace(cellPosition.x, cellPosition.y);
        Quaternion generatedChunkRotation = Quaternion.identity;

        CellStructure newStructure = Instantiate(cellStructuresTemplate, generatedChunkPosition, generatedChunkRotation, cellContainer);
        newStructure.Generate(cellData, CellScale);
        renderedCells.Add(cellPosition, newStructure);
    }

    protected virtual VirtualCellData InstantiateVirtualCell(Vector2Int cellPosition) => new VirtualCellData(cellPosition);

    protected virtual VirtualCellData CreateVirtualCell(Dictionary<Vector2Int, VirtualCellData> virtualCells, Vector2Int cellPosition)
    {
        VirtualCellData newVirtual = InstantiateVirtualCell(cellPosition);
        newVirtual.PreGenerate();
        virtualCells.Add(cellPosition, newVirtual);
        return newVirtual;
    }
}
