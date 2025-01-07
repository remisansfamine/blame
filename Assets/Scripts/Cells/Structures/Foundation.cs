using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Foundation : CellStructure
{
    [SerializeField] private ShapeRenderer shapeRendererTemplate;
    [SerializeField] private ShapeRenderer bridgeRendererTemplate;
    [SerializeField] private Vector3 maxDimensions = Vector3.one;

    private FoundationVirtualCellData virtualCellData;

    //private Bounds[] bounds;

    public override void Generate(VirtualCellData data, float cellScale)
    {
        virtualCellData = (FoundationVirtualCellData)data;
        Assert.IsNotNull(virtualCellData, "FATAL: Wrong VirtualCellData subclass type");

        //DetermineBottomAndTopHeights();
        GenerateShellMesh();

        foreach (FoundationVirtualCellData neighbor in virtualCellData.neighbors)
        {
            bool[] bridges = GetBridgesForEachFloor(data, neighbor);

            for (int i = 0; i < bridges.Length; i++)
            {
                if (bridges[i])
                {
                    float height = -maxDimensions.y * 0.5f + maxDimensions.y * (i / (float)bridges.Length);

                    Vector2 start = new Vector2(data.Position.x * cellScale, data.Position.y * cellScale);
                    Vector2 end = new Vector2(neighbor.Position.x * cellScale, neighbor.Position.y * cellScale);

                    GenerateBridgeMesh(start, end, height);
                }
            }
        }
    }

    //  NOTE : upgrade noise seed ?
    private float Noise(float noiseResolution, float seed = 0)
    {
        float noiseScale = 1f / noiseResolution;
        return Mathf.PerlinNoise(seed + transform.position.x * noiseScale, seed + transform.position.z * noiseScale);
    }

    private bool[] GetBridgesForEachFloor(VirtualCellData selfVirtualCell, VirtualCellData neighborVirtualCell, int floorCount = 10, float bridgePercentage = 0.25f)
    {
        bool[] result = new bool[floorCount];
        for (int i = 0; i < floorCount; i++)
        {
            float a = Mathf.Min(selfVirtualCell.Position.x, neighborVirtualCell.Position.x) + Mathf.Min(selfVirtualCell.Position.y, neighborVirtualCell.Position.y);
            float b = Mathf.Max(selfVirtualCell.Position.x, neighborVirtualCell.Position.x) + Mathf.Max(selfVirtualCell.Position.y, neighborVirtualCell.Position.y);
            float hash = HashCode.Combine(a, b, i) * 10e-9f;

            result[i] = Mathf.PerlinNoise1D(hash) < bridgePercentage;
        }
        return result;
    }

    //private void DetermineBottomAndTopHeights()
    //{
    //    const float TOP_HEIGHT_SEED = 1987.568f;
    //    const float BOT_HEIGHT_SEED = -7549.985f;

    //    float topHeight = (int)(Noise(1, TOP_HEIGHT_SEED) * 10) / 10f;
    //    float botHeight = (int)(Noise(1, BOT_HEIGHT_SEED) * 10) / 10f;
        
    //    if (topHeight + botHeight > 1f)
    //    {
    //        bounds = new[] {
    //            new Bounds(Vector3.zero, maxDimensions)
    //        };
    //        return;
    //    }
    //    Vector3 bottomDimensions = maxDimensions;
    //    bottomDimensions.y *= botHeight;

    //    Vector3 topDimensions = maxDimensions;
    //    topDimensions.y *= topHeight;

    //    bounds = new[] {
    //        new Bounds(Vector3.up * (-maxDimensions.y + bottomDimensions.y) * 0.5f , bottomDimensions),
    //        new Bounds(Vector3.up * (maxDimensions.y - topDimensions.y) * 0.5f , topDimensions)
    //    };
    //}

    private void GenerateShellMesh()
    {
        foreach (Bounds bound in virtualCellData.Bounds)
        {
            Mesh cubeMesh = bound.CreateCubeMeshFromBounds();
            ShapeRenderer shapeRenderer = Instantiate(shapeRendererTemplate, transform);
            shapeRenderer.Filter.mesh = cubeMesh; 
        }
    }


    private void GenerateBridgeMesh(Vector2 start, Vector2 end, float height)
    { 
        Vector3 direction = new Vector3(end.x - start.x, 0f, end.y - start.y);

        // Define vertices
        Vector3[] vertices = new Vector3[4];
        float width = .5f; // Width of the bridge
        Vector3 right = Vector3.Cross(direction.normalized, Vector3.up) * width * 0.5f;

        // Create a quad (4 vertices)
        Vector3 origin = Vector3.up * height;
        vertices[0] = origin - right;          
        vertices[1] = origin + right;
        vertices[2] = origin + direction * 0.5f - right;
        vertices[3] = origin + direction * 0.5f + right;

        // Define triangles
        int[] triangles = new int[6]
        {
        0, 2, 1,
        1, 2, 3
        };

        // Define UVs
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(0, 1);
        uvs[3] = new Vector2(1, 1);


        // Create the mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };

        mesh.RecalculateNormals(); // Calculate normals for lighting
        mesh.RecalculateBounds(); // Ensure bounds are set correctly

        ShapeRenderer shapeRenderer = Instantiate(bridgeRendererTemplate, transform);
        shapeRenderer.Filter.mesh = mesh;
    }

    private void DrawShape(Vector3 offset, Vector3 dimensions)
    {
        Gizmos.color = new Color(0, 0.75f, 0.75f, 0.25f);
        Gizmos.DrawCube(transform.position + offset, dimensions);

        Gizmos.color = new Color(0, 0.75f, 0.75f, 1f);
        Gizmos.DrawWireCube(transform.position + offset, dimensions);
    }

    //private void OnDrawGizmos()
    //{
    //    if (bounds == null)
    //        return;

    //    foreach (Bounds bound in bounds)
    //    {
    //        DrawShape(bound.center, bound.size);
    //    }
    //}

}
