using System.Collections.Generic;
using UnityEngine;

public class GeneratorComponent : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;
    [SerializeField] private Transform cellContainer = null;

    private HashSet<ChunkLoadComponent> chunkLoaders = new HashSet<ChunkLoadComponent>();
    private WorldCache cellsData = new WorldCache();

    private void OnDrawGizmos()
    {
        //foreach (var pair in cellsData.VirtualCells)
        //{
        //    foreach (VirtualCellData neighbor in pair.Value.neighbors)
        //    {
        //        Vector3 startPos = new Vector3(pair.Value.Position.x, 0, pair.Value.Position.y) * worldGenerator.CellScale;
        //        Vector3 endPos = new Vector3(neighbor.Position.x, 0, neighbor.Position.y) * worldGenerator.CellScale;

        //        Gizmos.color = new Color(0, 0.4f, 0.6f, 0.35f);
        //        Gizmos.DrawLine(startPos, endPos);
        //    }
        //}

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
    }

    public void RegisterChunkLoader(ChunkLoadComponent chunkLoader)
    {
        chunkLoaders.Add(chunkLoader);
    }

    public void UnregisterChunkLoader(ChunkLoadComponent chunkLoader)
    {
        chunkLoaders.Remove(chunkLoader);
    }
}
