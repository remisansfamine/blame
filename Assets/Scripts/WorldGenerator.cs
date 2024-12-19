using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerator", menuName = "Generators/WorldGenerator")]
public class WorldGenerator : ScriptableObject
{
    [SerializeField] private Vector2 offset;

    [SerializeField] private float cellScale = 1f;
    public float CellScale => cellScale;

    [SerializeField] private Grid worldGrid = new Grid();

    [SerializeField] private GameObject cellPrefab = null;
    
    [SerializeField, Range(0f, 1f)] private float spawnRate = 0.5f;

    [SerializeField] public float noiseResolution = 1f;

    [SerializeField] public int neighborhoodRange = 10;


    //private Dictionary<Vector2Int, GameObject> RenderedCells = new Dictionary<Vector2Int, GameObject>();

    void Generate()
    {

    }

    Vector2Int FromWorldSpaceToGridSpace(Vector3 WorldCoordinate)
    {
        return new Vector2Int(Mathf.CeilToInt(WorldCoordinate.x / CellScale), Mathf.CeilToInt(WorldCoordinate.z / CellScale)); 
    }

    Vector3 FromGridSpaceToWorldSpace(int x, int y)
    {
        return new Vector3(x * CellScale, 0f, y * CellScale);
    }

    float GetNoiseValueFromChunkSpace(float x, float y, float seed = 0)
    {
        float noiseScale = 1f / noiseResolution;
        return Mathf.PerlinNoise(seed + x * noiseScale + offset.x, seed + y * noiseScale + offset.y);
    }

    float GetNoiseValueFromWorldSpace(Vector2Int Coordinates)
    {
        return 0.0f;
    }


    public void UpdateCells(ChunkLoadComponent chunkLoader, CellsData cellsData, Transform cellContainer)
    {
        if (chunkLoader.OldPosition == chunkLoader.transform.position)
            return;

        Vector2Int loaderPosition = FromWorldSpaceToGridSpace(chunkLoader.transform.position);
        Vector2Int oldPosition2D = FromWorldSpaceToGridSpace(chunkLoader.OldPosition);

        Vector2Int deltaPosition = loaderPosition - oldPosition2D;

        float sqrMovement = deltaPosition.sqrMagnitude;

        Vector2Int extent = Vector2Int.one * chunkLoader.VirtualDistance;
        Rect newRect = new Rect(loaderPosition - extent, extent*2);

        //  if the distance traveled is higher than the virtual distance, clear all datas
        if (sqrMovement > chunkLoader.SquaredVirtualDistance * 4)
        {
            cellsData.ClearAll();
        }
        else
        {
            Rect oldRect = new Rect(oldPosition2D - extent, extent*2);
            Rect[] areaToClear = oldRect.Subtract(newRect);

            foreach (Rect rect in areaToClear)
            {
                rect.DrawRect(Color.red, 1);
                Vector2Int pos = Vector2Int.zero;
                for (pos.x = (int)rect.xMin; pos.x < (int)rect.xMax; pos.x++)
                {
                    for (pos.y = (int)rect.yMin; pos.y < (int)rect.yMax; pos.y++)
                    {
                        cellsData.ClearAt(pos);
                    }
                }
            }
        }

        Vector2Int currPosition = Vector2Int.zero;
        for (currPosition.x = (int)newRect.xMin; currPosition.x < (int)newRect.xMax; currPosition.x++)
        {
            for (currPosition.y = (int)newRect.yMin; currPosition.y < (int)newRect.yMax; currPosition.y++)
            {
                if (cellsData.IsCellOfState(currPosition, CellsData.CellState.EMPTY))
                    continue;

                //  Calculate square distance between current coordinate and chunk loader
                int DistSqr = currPosition.SqrdDistance(loaderPosition);

                if (DistSqr > chunkLoader.SquaredVirtualDistance)
                    continue;

                //  The cell is currently rendered
                if (cellsData.IsCellOfState(currPosition, CellsData.CellState.RENDERED))
                {
                    //  Should it be unloaded
                    if (DistSqr > chunkLoader.SquaredUnloadDistance)
                    {
                        UnloadRenderedCell(cellsData.RenderedCells, currPosition);
                    }
                }

                //  The cell is supposed to be rendered but is not rendered yet
                else if (cellsData.IsCellOfState(currPosition, CellsData.CellState.VIRTUAL))
                {
                    //  Should we render this cell ?
                    if (DistSqr <= chunkLoader.SquaredRenderDistance)
                        CreateRenderedCell(cellsData.RenderedCells, cellContainer, currPosition);
                }

                //  The cell has not been registered in any state, let's check if it should be filled depending on a noise
                else if (GetNoiseValueFromChunkSpace(currPosition.x, currPosition.y) >= 1f - spawnRate)
                {
                    CreateVirtualCell(cellsData.VirtualCells, currPosition);
                }

                //  Mark this cell as empty as it has no state yet and was not filled by the noise
                else
                {
                    cellsData.EmptyCells.Add(currPosition);
                }
            }
        }
    }

    private void UnloadRenderedCell(Dictionary<Vector2Int, GameObject> renderedCells, Vector2Int cellPosition)
    {
        Destroy(renderedCells[cellPosition]);
        renderedCells.Remove(cellPosition);
    }

    private void CreateRenderedCell(Dictionary<Vector2Int, GameObject> renderedCells, Transform cellContainer, Vector2Int cellPosition)
    {
        Vector3 generatedChunkPosition = FromGridSpaceToWorldSpace(cellPosition.x, cellPosition.y);
        Quaternion generatedChunkRotation = Quaternion.identity;
        renderedCells.Add(cellPosition, GameObject.Instantiate(cellPrefab, generatedChunkPosition, generatedChunkRotation, cellContainer));
    }

    private void CreateVirtualCell(Dictionary<Vector2Int, VirtualCell> virtualCells, Vector2Int cellPosition)
    {
        HashSet<VirtualCell> occupiedCellsInRange = virtualCells.Where(pair => pair.Key.SqrdDistance(cellPosition) <= neighborhoodRange * neighborhoodRange).Select(pair => pair.Value).ToHashSet();

        VirtualCell newVirtual = new VirtualCell(cellPosition);
        newVirtual.AddNeighbors(occupiedCellsInRange);
        virtualCells.Add(cellPosition, newVirtual);
    }
}
