using UnityEngine;

public class LevelDataHolder : Singleton<LevelDataHolder>
{
    [SerializeField] private LevelData data;
    public LevelData Data => data;
} 
