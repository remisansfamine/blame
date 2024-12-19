using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    private void OnDrawGizmos()
    {
        foreach (var pair in cellsData.VirtualCells)
        {
            foreach (VirtualCell neighbor in pair.Value.neighbors)
            {
                Vector3 startPos = new Vector3(pair.Value.Position.x, 0, pair.Value.Position.y) * worldGenerator.CellScale;
                Vector3 endPos = new Vector3(neighbor.Position.x, 0, neighbor.Position.y) * worldGenerator.CellScale;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(startPos, endPos);
            }
        }

        Matrix4x4 oldMatrix = Gizmos.matrix;
        foreach (ChunkLoadComponent chunkLoadComponent in chunkLoaders)
        {
            Gizmos.matrix = Matrix4x4.TRS(chunkLoadComponent.transform.position, Quaternion.identity, new Vector3(1, .01f, 1));
            Gizmos.color = new Color(.45f, .3f, .3f, .35f);
            Gizmos.DrawSphere(Vector3.zero, chunkLoadComponent.VirtualDistance * worldGenerator.CellScale);
            Gizmos.color = new Color(.0f, 0.95f, .70f, .35f);
            Gizmos.DrawSphere(Vector3.zero, chunkLoadComponent.RenderDistance * worldGenerator.CellScale);
        }
        Gizmos.matrix = oldMatrix;
    }

    void Update()
    {
        foreach (ChunkLoadComponent chunkLoader in chunkLoaders)
        {
            worldGenerator.UpdateCells(chunkLoader, cellsData, cellContainer);
        }


        //  Draw debugs
        foreach (var pair in cellsData.VirtualCells)
        {
            foreach (VirtualCell neighbor in pair.Value.neighbors)
            {
                Vector3 startPos = new Vector3(pair.Value.Position.x, 0, pair.Value.Position.y) * worldGenerator.CellScale;
                Vector3 endPos = new Vector3(neighbor.Position.x, 0, neighbor.Position.y) * worldGenerator.CellScale;
                Debug.DrawLine(startPos, endPos, Color.green);
            }
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
