using UnityEngine;

public class ChunkLoadComponent : MonoBehaviour
{
    [SerializeField] private int virtualDistance;
    [SerializeField] private int renderDistance;

    public int VirtualDistance => virtualDistance;
    public int RenderDistance => renderDistance;
}
