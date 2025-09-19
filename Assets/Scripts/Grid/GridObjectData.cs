using System.Collections.Generic;
using UnityEngine;

public class GridObjectData : ScriptableObject
{
    public string objectName;
    public int id;
    public GameObject prefab;

    public override string ToString() => id.ToString();
    
}
