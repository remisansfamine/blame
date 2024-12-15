using UnityEngine;

public class ChunkLoadComponent : MonoBehaviour
{
    [SerializeField] private int virtualDistance;
    [SerializeField] private int renderDistance;
    [SerializeField] private int unloadDistance;

    public int VirtualDistance => virtualDistance;
    public int RenderDistance => renderDistance;
    public int UnloadDistance => unloadDistance;
}
