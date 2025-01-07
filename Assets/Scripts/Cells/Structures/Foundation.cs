using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Foundation : CellStructure
{
    [SerializeField] private ShapeRenderer shapeRendererTemplate;
    [SerializeField] private ShapeRenderer bridgeRendererTemplate;
    [SerializeField] private Vector3 maxDimensions = Vector3.one;

    private FoundationVirtualCellData virtualCellData;


    public override void Generate(VirtualCellData data, float cellScale)
    {
        virtualCellData = (FoundationVirtualCellData)data;
        Assert.IsNotNull(virtualCellData, "FATAL: Wrong VirtualCellData subclass type");

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

    private void GenerateShellMesh()
    {
        foreach (Bounds bound in virtualCellData.Bounds)
        {
            GenerateFromBound(bound);

            //Mesh cubeMesh = bound.CreateCubeMeshFromBounds();
            //ShapeRenderer shapeRenderer = Instantiate(shapeRendererTemplate, transform);
            //shapeRenderer.Filter.mesh = cubeMesh;
        }
    }


    private void GenerateFromBound(Bounds bound)
    {
        //Vector3 scale = new Vector3(1f / bound.size.x, 1f / bound.size.y, 1f / bound.size.z);

        for (float y = -bound.extents.y; y < bound.extents.y; y++)
        {
            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;
                GameObject go = Instantiate(assetsPack.Walls[0], transform);
                go.transform.localPosition = new Vector3(xPos, y, bound.extents.z) + bound.center;
            }

            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;
                GameObject go = Instantiate(assetsPack.Walls[0], transform);
                go.transform.localPosition = new Vector3(xPos, y, -bound.extents.z) + bound.center;
                go.transform.forward = Vector3.back;
            }

            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;
                GameObject go = Instantiate(assetsPack.Walls[0], transform);
                go.transform.localPosition = new Vector3(bound.extents.x, y, zPos) + bound.center;
                go.transform.forward = Vector3.right;
            }


            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;
                GameObject go = Instantiate(assetsPack.Walls[0], transform);
                go.transform.localPosition = new Vector3(-bound.extents.x, y, zPos) + bound.center;
                go.transform.forward = Vector3.left;
            }
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

    private void OnDrawGizmos()
    {
        if (virtualCellData.Bounds == null)
            return;

        foreach (Bounds bound in virtualCellData.Bounds)
        {
            DrawShape(bound.center, bound.size);
        }
    }

}
