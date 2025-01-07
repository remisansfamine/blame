using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VirtualCellData : CellData
{
    public VirtualCellData(Vector2Int position) : base(position)
    { }

    public virtual void PreGenerate() { }
}
