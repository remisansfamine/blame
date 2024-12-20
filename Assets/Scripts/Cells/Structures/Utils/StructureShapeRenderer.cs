using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class StructureShapeRenderer : MonoBehaviour
{
    public MeshRenderer Renderer { get; private set; }
    public MeshFilter Filter { get; private set; }

    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
        Filter = GetComponent<MeshFilter>();
    }
}
