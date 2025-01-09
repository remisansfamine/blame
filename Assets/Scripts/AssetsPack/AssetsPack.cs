using UnityEngine;

[CreateAssetMenu(fileName = "AssetsPack", menuName = "Assets Pack")]
public class AssetsPack : ScriptableObject
{
    public Mesh[] Doors;
    public Mesh[] Walls;
    public Mesh[] Floors;
    public Mesh[] Ceilings;


    public GameObject[] WallsGo;
}
