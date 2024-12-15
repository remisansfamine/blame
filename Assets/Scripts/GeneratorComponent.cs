using System.Collections.Generic;
using UnityEngine;

public class GeneratorComponent : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;


    [SerializeField] private Transform cameraTransform;
    [SerializeField, Range(0f, 250f)] private float virtualGenerationDistance = 10f;
    [SerializeField] private Transform cellContainer = null;

    [SerializeField] private ChunkLoadComponent DEBUG_ChunkLoader = null;
    private List<ChunkLoadComponent> chunkLoaders = new List<ChunkLoadComponent>();

    CellsData cellsData = new CellsData();

    void Start()
    {
        RegisterChunkLoader(DEBUG_ChunkLoader);
    }

    void FixedUpdate()
    {
        foreach (ChunkLoadComponent chunkLoader in chunkLoaders)
        {
            worldGenerator.UpdateCells(chunkLoader, cellsData, cellContainer);
        }
    }

    public void RegisterChunkLoader(ChunkLoadComponent chunkLoader)
    {
        if (!chunkLoaders.Contains(chunkLoader))
            chunkLoaders.Add(chunkLoader);
    }

    public void UnregisterChunkLoader(ChunkLoadComponent chunkLoader)
    {
        if (chunkLoaders.Contains(chunkLoader))
            chunkLoaders.Remove(chunkLoader);
    }
}
