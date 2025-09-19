using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Grid/Buildable Object Data")]
public class BuildableObjectData : GridObjectData
{
    [SerializeField]
    private List<ResourceCost> materialCosts;
    public Dictionary<ResourceData, int> CostDictionary => materialCosts.ToDictionary(mc => mc.material, mc => mc.amount);

    [Serializable]
    public class ResourceCost
    {
        public ResourceData material;
        public int amount;
    }
    public int CalculateGoldValue()
    {
        int totalGold = 0;

        foreach (var cost in materialCosts)
        {
            if (cost.material != null)
                totalGold += cost.material.coinValue * cost.amount;
            else
            {
                Debug.LogWarning($"Null material in {name}'s materialCosts list.");
            }
        }

        return totalGold;
    }
}