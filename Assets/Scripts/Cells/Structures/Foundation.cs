//#define USE_SINGLE_GO

using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

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

        Vector2 selfCenter = new Vector2(data.Position.x * cellScale, data.Position.y * cellScale);
        Rect selfRect = new Rect(selfCenter - 0.5f * Vector2.one, Vector2.one);

        int floorCount = 10;
        for (int i = 0; i< floorCount; i++)
        {
            float height = -maxDimensions.y * 0.5f + maxDimensions.y * (i / (float)floorCount);
            if (virtualCellData.IsValidHeight(height))
            {
                foreach (FoundationVirtualCellData neighbor in virtualCellData.neighbors)
                {
                    if (neighbor.IsValidHeight(height) && IsBridgeAtFloor(virtualCellData, neighbor, i))
                    {
                        Vector2 neighborCenter = new Vector2(neighbor.Position.x * cellScale, neighbor.Position.y * cellScale);

                        Rect neighborRect = new Rect(neighborCenter - 0.5f * Vector2.one, Vector2.one);
                        GenerateBridgeMesh(selfRect.ClampPosition(neighborCenter), neighborRect.ClampPosition(selfCenter), height);
                    }
                }
            }
        }
    }

    private bool IsBridgeAtFloor(VirtualCellData selfVirtualCell, VirtualCellData neighborVirtualCell, int floor, float bridgeSpawnRate = 0.25f)
    {
        float a = Mathf.Min(selfVirtualCell.Position.x, neighborVirtualCell.Position.x) + Mathf.Min(selfVirtualCell.Position.y, neighborVirtualCell.Position.y);
        float b = Mathf.Max(selfVirtualCell.Position.x, neighborVirtualCell.Position.x) + Mathf.Max(selfVirtualCell.Position.y, neighborVirtualCell.Position.y);
        float hash = HashCode.Combine(a, b, floor) * 10e-9f;

        return Mathf.PerlinNoise1D(hash) < bridgeSpawnRate;
    }

    private void GenerateShellMesh()
    {
        foreach (Bounds bound in virtualCellData.Bounds)
        {
            GenerateFromBound(bound);
        }
    }


    private void GenerateFromBound(Bounds bound)
    {

    #if !USE_SINGLE_GO
        List<CombineInstance> combineInstances = new List<CombineInstance>();
    #endif


        for (float y = -bound.extents.y; y < bound.extents.y; y++)
        {

    #if USE_SINGLE_GO
            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;
                GameObject go = Instantiate(assetsPack.WallsGo[0], transform);
                go.transform.localPosition = new Vector3(xPos, y, bound.extents.z) + bound.center;
            }

            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;
                GameObject go = Instantiate(assetsPack.WallsGo[0], transform);
                go.transform.localPosition = new Vector3(xPos, y, -bound.extents.z) + bound.center;
                go.transform.forward = Vector3.back;
            }

            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;
                GameObject go = Instantiate(assetsPack.WallsGo[0], transform);
                go.transform.localPosition = new Vector3(bound.extents.x, y, zPos) + bound.center;
                go.transform.forward = Vector3.right;
            }


            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;
                GameObject go = Instantiate(assetsPack.WallsGo[0], transform);
                go.transform.localPosition = new Vector3(-bound.extents.x, y, zPos) + bound.center;
                go.transform.forward = Vector3.left;
            }
    #else

            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;

                CombineInstance instance = new CombineInstance();
                instance.mesh = assetsPack.Walls[0];

                Vector3 position = new Vector3(xPos, y + 0.5f, bound.extents.z - 0.025f) + bound.center;
                Quaternion rotation = Quaternion.identity;
                Vector3 scale = new Vector3(1f, 1f, 0.05f);

                instance.transform = Matrix4x4.TRS(position, rotation, scale);
                combineInstances.Add(instance);
            }

            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;

                CombineInstance instance = new CombineInstance();
                instance.mesh = assetsPack.Walls[0];

                Vector3 position = new Vector3(xPos, y + 0.5f, -bound.extents.z + 0.025f) + bound.center;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.back);
                Vector3 scale = new Vector3(1f, 1f, 0.05f);

                instance.transform = Matrix4x4.TRS(position, rotation, scale);
                combineInstances.Add(instance);
            }

            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;

                CombineInstance instance = new CombineInstance();
                instance.mesh = assetsPack.Walls[0];

                Vector3 position = new Vector3(bound.extents.x - 0.025f, y + 0.5f, zPos) + bound.center;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
                Vector3 scale = new Vector3(1f, 1f, 0.05f);

                instance.transform = Matrix4x4.TRS(position, rotation, scale);
                combineInstances.Add(instance);
            }


            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;

                CombineInstance instance = new CombineInstance();
                instance.mesh = assetsPack.Walls[0];

                Vector3 position = new Vector3(-bound.extents.x + 0.025f, y + 0.5f, zPos) + bound.center;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.left);
                Vector3 scale = new Vector3(1f, 1f, 0.05f);

                instance.transform = Matrix4x4.TRS(position, rotation, scale);
                combineInstances.Add(instance);
            }
    #endif

        }


    #if !USE_SINGLE_GO
        ShapeRenderer render = Instantiate(shapeRendererTemplate, transform);
            Mesh mergedMesh = new Mesh();
            mergedMesh.CombineMeshes(combineInstances.ToArray());
            mergedMesh.Optimize();
            render.Filter.sharedMesh = mergedMesh;
    #endif
    }

    private void GenerateBridgeMesh(Vector2 start, Vector2 end, float height)
    {
        Vector2 direction2D = end - start;
        float length = direction2D.magnitude;

        direction2D.Normalize();

        // Define vertices
        Vector3[] vertices = new Vector3[4];
        float width = .5f; // Width of the bridge

        Vector3 start3D = new Vector3(start.x, 0f, start.y);
        Vector3 direction = new Vector3(direction2D.x, 0f, direction2D.y);
        Vector3 right = Vector3.Cross(direction, Vector3.up) * width * 0.5f;

        // Create a quad (4 vertices)
        Vector3 origin = Vector3.up * height + (start3D - transform.position);
        vertices[0] = origin - right;          
        vertices[1] = origin + right;
        vertices[2] = origin + direction * length * 0.5f - right;
        vertices[3] = origin + direction * length * 0.5f + right;

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
        //if (virtualCellData.Bounds == null)
        //    return;

        //foreach (Bounds bound in virtualCellData.Bounds)
        //{
        //    DrawShape(bound.center, bound.size);
        //}
    }

}
