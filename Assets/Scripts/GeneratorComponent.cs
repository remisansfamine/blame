using UnityEngine;

public class GeneratorComponent : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;

    [SerializeField] private Transform cameraTransform;
    [SerializeField, Range(0f, 250f)] private float virtualGenerationDistance = 10f;
    [SerializeField] private Transform cellContainer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldGenerator.UpdateVirtualMap(cameraTransform, virtualGenerationDistance, cellContainer);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
