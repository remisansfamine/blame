using UnityEngine;

public static class BoundsUtils
{
    public static Mesh CreateCubeMeshFromBounds(this Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // Define the 8 vertices of the cube
        Vector3[] vertices = new Vector3[]
        {
            center + new Vector3(-extents.x, -extents.y, -extents.z), // Bottom-back-left
            center + new Vector3(extents.x, -extents.y, -extents.z),  // Bottom-back-right
            center + new Vector3(extents.x, -extents.y, extents.z),   // Bottom-front-right
            center + new Vector3(-extents.x, -extents.y, extents.z),  // Bottom-front-left
            center + new Vector3(-extents.x, extents.y, -extents.z),  // Top-back-left
            center + new Vector3(extents.x, extents.y, -extents.z),   // Top-back-right
            center + new Vector3(extents.x, extents.y, extents.z),    // Top-front-right
            center + new Vector3(-extents.x, extents.y, extents.z)    // Top-front-left
        };

        // Define the triangles (2 per face, 6 faces)
        int[] triangles = new int[]
        {
            // Bottom face
            0, 2, 1,
            0, 3, 2,
            // Top face
            4, 5, 6,
            4, 6, 7,
            // Front face
            3, 6, 2,
            3, 7, 6,
            // Back face
            0, 1, 5,
            0, 5, 4,
            // Left face
            0, 4, 7,
            0, 7, 3,
            // Right face
            1, 2, 6,
            1, 6, 5
        };

        // Define the UVs (optional, for texturing)
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
        };

        // Create the mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };

        mesh.RecalculateNormals(); // Calculate normals for lighting
        mesh.RecalculateBounds(); // Ensure bounds are set correctly

        return mesh;
    }
}
