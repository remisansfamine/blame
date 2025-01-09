using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoundationVirtualCellData : VirtualCellData
{
    private Bounds[] bounds;

    //  TODO : to change later
    private Vector3 dimensions = new Vector3(1, 20, 1);

    public Bounds[] Bounds => bounds;

    public HashSet<VirtualCellData> neighbors = new HashSet<VirtualCellData>();

    public FoundationVirtualCellData(Vector2Int position) : base(position)
    {
    }

    public virtual void AddNeighbor(VirtualCellData newNeighbor)
    {
        neighbors.Add(newNeighbor);

        FoundationVirtualCellData foundationVirtualCellData = newNeighbor as FoundationVirtualCellData;
        foundationVirtualCellData.neighbors.Add(this);
    }

    public virtual void AddNeighbors(HashSet<VirtualCellData> newNeighbors)
    {
        neighbors.AddRange(newNeighbors);

        foreach (FoundationVirtualCellData neighbor in neighbors)
            neighbor.neighbors.Add(this);
    }

    private float Noise(float noiseResolution, float seed = 0)
    {
        float noiseScale = 1f / noiseResolution;
        return Mathf.PerlinNoise(seed + Position.x * noiseScale, seed + Position.y * noiseScale);
    }

    private void DetermineBottomAndTopHeights()
    {
        const float TOP_HEIGHT_SEED = 1987.568f;
        const float BOT_HEIGHT_SEED = -7549.985f;

        float topHeight = (int)(Noise(1, TOP_HEIGHT_SEED) * 10) / 10f;
        float botHeight = (int)(Noise(1, BOT_HEIGHT_SEED) * 10) / 10f;

        if (topHeight + botHeight > 1f)
        {
            bounds = new[] {
                new Bounds(Vector3.zero, dimensions)
            };
            return;
        }
        Vector3 bottomDimensions = dimensions;
        bottomDimensions.y *= botHeight;

        Vector3 topDimensions = dimensions;
        topDimensions.y *= topHeight;

        bounds = new[] {
            new Bounds(Vector3.up * (-dimensions.y + bottomDimensions.y) * 0.5f , bottomDimensions),
            new Bounds(Vector3.up * (dimensions.y - topDimensions.y) * 0.5f , topDimensions)
        };
    }

    public override void PreGenerate() 
    {
        DetermineBottomAndTopHeights();
    }


    public bool IsValidHeight(float height)
    {
        foreach (Bounds bound in bounds)
        {
            if (height > bound.min.y && height < bound.max.y)
                return true;
        }

        return false;
    }



    //  TODO : to change later
    public Rect GetCellLocalRect() => new Rect(Vector2.zero, Vector2.one);
}
