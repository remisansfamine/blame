using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoundationVirtualCellData : VirtualCellData
{
    private Bounds[] bounds;

    public FoundationVirtualCellData(Vector2Int position) : base(position)
    {
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
                new Bounds(Vector3.zero, maxDimensions)
            };
            return;
        }
        Vector3 bottomDimensions = maxDimensions;
        bottomDimensions.y *= botHeight;

        Vector3 topDimensions = maxDimensions;
        topDimensions.y *= topHeight;

        bounds = new[] {
            new Bounds(Vector3.up * (-maxDimensions.y + bottomDimensions.y) * 0.5f , bottomDimensions),
            new Bounds(Vector3.up * (maxDimensions.y - topDimensions.y) * 0.5f , topDimensions)
        };
    }

    public override void PreGenerate() 
    {

    }
}
