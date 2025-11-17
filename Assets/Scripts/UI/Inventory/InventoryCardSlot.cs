using UnityEngine;

public class InventoryCardSlot : MonoBehaviour
{
    public bool IsOccupied => transform.childCount > 0;
    public SkillCardUI GetCardUI()
    {
        if (IsOccupied)
            return transform.GetChild(0).GetComponent<SkillCardUI>();
        return null;
    }
}
