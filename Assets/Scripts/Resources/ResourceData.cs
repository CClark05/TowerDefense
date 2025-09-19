using UnityEngine;


[CreateAssetMenu(fileName = "ResourceData")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public GameObject prefab;
    public int coinValue;
    public int maxResources;
}
