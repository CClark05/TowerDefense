using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Resource : MonoBehaviour, IHoverable
{
    [FormerlySerializedAs("materialTypeData")] [SerializeField] private ResourceData resourceData;
    public ResourceData ResourceData => resourceData;
    
    private int currentResourceAmount;
    public event Action OnHoverEvent;
    public event Action OnHoverLeaveEvent;
    public static event Action<ResourceData, int> OnDroppedMaterial;
    public static event Action<Vector2Int> OnDeath;
    
    private Vector2Int gridPosition;
    public bool IgnoreRange => false;
    public void OnHover() => OnHoverEvent?.Invoke();

    public void OnLeaveHover() => OnHoverLeaveEvent?.Invoke();

    private void Start()
    {
       GridManager.Instance.Grid.GetXY(transform.position, out int x, out int y);
       currentResourceAmount = resourceData.maxResources;
       gridPosition = new Vector2Int(x, y);
    }

    public void OnClick()
    {
        int maxHarvestable = Mathf.Min(3, currentResourceAmount); 
        int amountHarvested = UnityEngine.Random.Range(1, maxHarvestable + 1); 
        currentResourceAmount -= amountHarvested;
        OnDroppedMaterial?.Invoke(resourceData,amountHarvested);
        if (currentResourceAmount <= 0)
        {
            OnLeaveHover();
            OnDeath?.Invoke(gridPosition);
            Destroy(gameObject);
        }
    }

    
}
