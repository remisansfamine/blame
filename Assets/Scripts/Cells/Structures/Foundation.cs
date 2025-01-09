//#define USE_SINGLE_GO

using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class Foundation : CellStructure
{
    [SerializeField] private ShapeRenderer shapeRendererTemplate;
    [SerializeField] private ShapeRenderer bridgeRendererTemplate;
    [SerializeField] private Material[] materials;

    private FoundationVirtualCellData virtualCellData;


    public override void Generate(VirtualCellData data, float cellScale)
    {
        virtualCellData = (FoundationVirtualCellData)data;
        Assert.IsNotNull(virtualCellData, "FATAL: Wrong VirtualCellData subclass type");

        GenerateShellMesh();

        Vector2 selfCenter = new Vector2(data.Position.x * cellScale, data.Position.y * cellScale);
        Rect selfRect = new Rect(selfCenter - 0.5f * Vector2.one, Vector2.one);

        int floorCount = 25;
        for (int i = 0; i< floorCount; i++)
        {
            float height = -virtualCellData.Dimensions.y * 0.5f + virtualCellData.Dimensions.y * (i / (float)floorCount);
            if (virtualCellData.IsValidHeight(height))
            {
                foreach (FoundationVirtualCellData neighbor in virtualCellData.neighbors)
                {
                    if (neighbor.IsValidHeight(height) && IsBridgeAtFloor(virtualCellData, neighbor, i))
                    {
                        Vector2 neighborCenter = new Vector2(neighbor.Position.x * cellScale, neighbor.Position.y * cellScale);

                        Rect neighborRect = new Rect(neighborCenter - 0.5f * Vector2.one, Vector2.one);
                        Vector2 direction = (neighborCenter - selfCenter).normalized;
                        GenerateBridgeMesh(selfRect.PointOnSurface(direction), neighborRect.PointOnSurface(-direction), height);
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


    private Mesh GetWallFromInfo(Vector3 localPosition,int direction)
    {
        int hash = HashCode.Combine(localPosition.x, localPosition.y, localPosition.z, direction);
        int index = Mathf.Abs(hash) % assetsPack.Walls.Length;
        return assetsPack.Walls[index];
    }

private void GenerateFromBound(Bounds bound)
    {

        List<List<CombineInstance>> combineInstances = new List<List<CombineInstance>>();

        for (int i = 0; i < assetsPack.Walls.Length; i++)
        {
            if (combineInstances.Count < assetsPack.Walls[i].subMeshCount)
                combineInstances.Add(new List<CombineInstance>());
        }

        for (float y = -bound.extents.y; y < bound.extents.y; y++)
        {
            for (float x = -bound.extents.x; x < bound.extents.x; x++)
            {
                float xPos = x + 0.5f;

                {
                    Vector3 position = new Vector3(xPos, y, bound.extents.z + 0.5f) + bound.center;
                    Quaternion rotation = Quaternion.identity;

                    CombineInstance instance = new CombineInstance();
                    instance.mesh = GetWallFromInfo(position, 0);
                    instance.transform = Matrix4x4.TRS(position, rotation, Vector3.one); 

                    for (int i = 0; i < instance.mesh.subMeshCount; i++)
                    {
                        instance.subMeshIndex = i;
                        combineInstances[i].Add(instance);
                    }
                }

                {
                    Vector3 position = new Vector3(xPos, y, -bound.extents.z - 0.5f) + bound.center;
                    Quaternion rotation = Quaternion.AngleAxis(180f, Vector3.up);

                    CombineInstance instance = new CombineInstance();
                    instance.mesh = GetWallFromInfo(position, 1);
                    instance.transform = Matrix4x4.TRS(position, rotation, Vector3.one);

                    for (int i = 0; i < instance.mesh.subMeshCount; i++)
                    {
                        instance.subMeshIndex = i;
                        combineInstances[i].Add(instance);
                    }
                }
            }

            for (float z = -bound.extents.z; z < bound.extents.z; z++)
            {
                float zPos = z + 0.5f;

                {
                    Vector3 position = new Vector3(bound.extents.x + 0.5f, y, zPos) + bound.center;
                    Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.up);

                    CombineInstance instance = new CombineInstance();
                    instance.mesh = GetWallFromInfo(position, 2);
                    instance.transform = Matrix4x4.TRS(position, rotation, Vector3.one);

                    for (int i = 0; i < instance.mesh.subMeshCount; i++)
                    {
                        instance.subMeshIndex = i;
                        combineInstances[i].Add(instance);
                    }
                }

                {
                    Vector3 position = new Vector3(-bound.extents.x - 0.5f, y, zPos) + bound.center;
                    Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);

                    CombineInstance instance = new CombineInstance();
                    instance.mesh = GetWallFromInfo(position, 3);
                    instance.transform = Matrix4x4.TRS(position, rotation, Vector3.one);

                    for (int i = 0; i < instance.mesh.subMeshCount; i++)
                    {
                        instance.subMeshIndex = i;
                        combineInstances[i].Add(instance);
                    }
                }
            }
        }

        for (int i = 0; i < combineInstances.Count; i++)
        {
            ShapeRenderer render = Instantiate(shapeRendererTemplate, transform);
            Mesh mergedMesh = new Mesh();
            mergedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mergedMesh.CombineMeshes(combineInstances[i].ToArray());
            mergedMesh.Optimize();
            render.Filter.sharedMesh = mergedMesh;
            render.Renderer.material = materials[i];
        }
    }

    private void GenerateBridgeMesh(Vector2 start, Vector2 end, float height)
    {
        Vector2 direction2D = end - start;
        float halfLength = direction2D.magnitude * 0.5f;

        direction2D.Normalize();

        float width = .5f;

        Vector3 start3D = new Vector3(start.x, 0f, start.y);
        Vector3 direction = new Vector3(direction2D.x, 0f, direction2D.y);
        Vector3 right = Vector3.Cross(direction, Vector3.up) * width * 0.5f;

        Vector3 origin = Vector3.up * height + (start3D - transform.position);

        // Create a quad (4 vertices)
        Vector3[] vertices = new Vector3[4];
        vertices[0] = origin - right;          
        vertices[1] = origin + right;
        vertices[2] = origin + direction * halfLength - right;
        vertices[3] = origin + direction * halfLength + right;

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
