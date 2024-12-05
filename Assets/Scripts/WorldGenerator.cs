using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldGenerator", menuName = "Generators/WorldGenerator")]
public class WorldGenerator : ScriptableObject
{
    [SerializeField] private Vector2 offset;

    [SerializeField] private float cellScale = 1f;
    public float CellScale => cellScale;

    [SerializeField] private Grid worldGrid = new Grid();

    [SerializeField] private GameObject cellPrefab = null;
    
    [SerializeField, Range(0f, 1f)] private float spawnRate = 0.5f;

    [SerializeField] public float noiseResolution = 1f;

    void Generate()
    {

    }

    Vector2Int FromWorldSpaceToGridSpace(Vector3 WorldCoordinate)
    {
        return new Vector2Int(Mathf.CeilToInt(WorldCoordinate.x / CellScale), Mathf.CeilToInt(WorldCoordinate.z / CellScale)); 
    }

    Vector3 FromGridSpaceToWorldSpace(int x, int y)
    {
        return new Vector3(x * CellScale, 0f, y * CellScale);
    }

    float GetNoiseValueFromChunkSpace(float x, float y)
    {
        float noiseScale = 1f / noiseResolution;
        return Mathf.PerlinNoise(x * noiseScale + offset.x, y * noiseScale + offset.y);
    }

    float GetNoiseValueFromWorldSpace(Vector2Int Coordinates)
    {
        return 0.0f;
    }

    public void UpdateVirtualMap(Transform cameraTransform, float virtualGenerationDistance, Transform cellContainer)
    {
        Vector2Int camChunkPosition = FromWorldSpaceToGridSpace(cameraTransform.position);

        int halfExtent = (int)(virtualGenerationDistance);
        for (int x = camChunkPosition.x - halfExtent; x < camChunkPosition.x + halfExtent; x++)
        {
            for (int y = camChunkPosition.y - halfExtent; y < camChunkPosition.y + halfExtent; y++)
            {
                float noiseValue = GetNoiseValueFromChunkSpace(x, y);
                Debug.Log($"{new Vector2Int(x,y)} -> {noiseValue}");
                if (noiseValue >= 1f - spawnRate)
                {
                    Vector3 generatedChunkPosition = FromGridSpaceToWorldSpace(x, y);
                    Quaternion generatedChunkRotation = Quaternion.identity;
                    GameObject.Instantiate(cellPrefab, generatedChunkPosition, generatedChunkRotation, cellContainer);
                }
            }
        }
    }
}
