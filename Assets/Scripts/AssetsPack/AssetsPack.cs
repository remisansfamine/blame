using UnityEngine;

[CreateAssetMenu(fileName = "AssetsPack", menuName = "Assets Pack")]
public class AssetsPack : ScriptableObject
{
    public GameObject[] Doors;
    public GameObject[] Walls;
    public GameObject[] Floors;
    public GameObject[] Ceilings;
}
