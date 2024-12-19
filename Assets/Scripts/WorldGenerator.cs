using System;
using System.Collections.Generic;
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


    private Dictionary<Vector2Int, GameObject> RenderedCells = new Dictionary<Vector2Int, GameObject>();

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


    public void UpdateVirtualMap(Transform cameraTransform, float virtualGenerationDistance, Transform cellContainer)
    {
        Vector2Int camChunkPosition = FromWorldSpaceToGridSpace(cameraTransform.position);

        int halfExtent = (int)(virtualGenerationDistance);
        for (int x = camChunkPosition.x - halfExtent; x < camChunkPosition.x + halfExtent; x++)
        {
            for (int y = camChunkPosition.y - halfExtent; y < camChunkPosition.y + halfExtent; y++)
            {
                float noiseValue = GetNoiseValueFromChunkSpace(x, y);
                Debug.Log($"{new Vector2Int(x,y)} -> {noiseValue}");
                if (noiseValue >= 1f - spawnRate)
                {
                    Vector3 generatedChunkPosition = FromGridSpaceToWorldSpace(x, y);
                    Quaternion generatedChunkRotation = Quaternion.identity;
                    GameObject.Instantiate(cellPrefab, generatedChunkPosition, generatedChunkRotation, cellContainer);
                }
            }
        }
    }


    public void UpdateRenderedMap(Transform cameraTransform, float renderGenerationDistance, Transform cellContainer)
    {

    }

    public void UpdateCells(ChunkLoadComponent chunkLoader, CellsData cellsData, Transform cellContainer)
    {
        Vector2Int LoaderPosition = FromWorldSpaceToGridSpace(chunkLoader.transform.position);

        float sqrUnloadDistance = chunkLoader.UnloadDistance * chunkLoader.UnloadDistance;
        float sqrRenderDistance = chunkLoader.RenderDistance * chunkLoader.RenderDistance;

        Vector2Int Coordinate = Vector2Int.zero;
        for (Coordinate.x = LoaderPosition.x - chunkLoader.VirtualDistance; Coordinate.x < LoaderPosition.x + chunkLoader.VirtualDistance; Coordinate.x++)
        {
            for (Coordinate.y = LoaderPosition.y - chunkLoader.VirtualDistance; Coordinate.y < LoaderPosition.y + chunkLoader.VirtualDistance; Coordinate.y++)
            {
                if (cellsData.IsCellOfState(Coordinate, CellsData.CellState.EMPTY))
                    continue;

                //  Calculate square distance between current coordinate and chunk loader
                float DistSqr = (LoaderPosition.x - Coordinate.x) * (LoaderPosition.x - Coordinate.x) +
                                (LoaderPosition.y - Coordinate.y) * (LoaderPosition.y - Coordinate.y);

                //  The cell is currently rendered
                if (cellsData.IsCellOfState(Coordinate, CellsData.CellState.RENDERED))
                {
                    //  Should it be unloaded
                    if (DistSqr > sqrUnloadDistance)
                    {
                        //  NOTE : Is dictionary the best solution ?
                        Destroy(RenderedCells[Coordinate]);
                        RenderedCells.Remove(Coordinate);

                        //  Switch from rendered to virtual
                        cellsData.UnregisterCellByState(Coordinate, CellsData.CellState.RENDERED);
                        cellsData.RegisterCellByState(Coordinate, CellsData.CellState.VIRTUAL);
                    }
                }

                //  The cell is supposed to be rendered but is not rendered yet
                else if (cellsData.IsCellOfState(Coordinate, CellsData.CellState.VIRTUAL))
                {
                    //  Should we render this cell ?
                    if (DistSqr <= sqrRenderDistance)
                    {
                        Vector3 generatedChunkPosition = FromGridSpaceToWorldSpace(Coordinate.x, Coordinate.y);
                        Quaternion generatedChunkRotation = Quaternion.identity;

                        //  NOTE : Is dictionary the best solution ?
                        RenderedCells.Add(Coordinate, GameObject.Instantiate(cellPrefab, generatedChunkPosition, generatedChunkRotation, cellContainer));

                        //  Switch from virtuaal to rendered
                        cellsData.UnregisterCellByState(Coordinate, CellsData.CellState.VIRTUAL);
                        cellsData.RegisterCellByState(Coordinate, CellsData.CellState.RENDERED);
                    }
                }

                //  The cell has not been registered in any state, let's check if it should be filled depending on a noise
                else if (GetNoiseValueFromChunkSpace(Coordinate.x, Coordinate.y) >= 1f - spawnRate)
                {
                    cellsData.RegisterCellByState(Coordinate, CellsData.CellState.VIRTUAL);
                }

                //  Mark this cell as empty as it has no state yet and was not filled by the noise
                else
                {
                    cellsData.RegisterCellByState(Coordinate, CellsData.CellState.EMPTY);
                }
            }
        }
    }
}
