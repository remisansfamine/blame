using UnityEngine;

public class ChunkLoadComponent : MonoBehaviour
{
    [SerializeField] private int virtualDistance;
    [SerializeField] private int renderDistance;
    [SerializeField] private int unloadDistance;

    public Vector3 OldPosition { get; private set; }

    public int VirtualDistance => virtualDistance;
    public int RenderDistance => renderDistance;
    public int UnloadDistance => unloadDistance;

    public int SquaredVirtualDistance => virtualDistance * virtualDistance;
    public int SquaredRenderDistance => renderDistance * renderDistance;
    public int SquaredUnloadDistance => unloadDistance * unloadDistance;

    private void LateUpdate()
    {
        OldPosition = transform.position;
    }
}
