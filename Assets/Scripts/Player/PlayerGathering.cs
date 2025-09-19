using System;
using UnityEngine;

public class PlayerGathering : MonoBehaviour
{
    private PlayerCursor cursor;
    public event Action OnGather;
    private void Awake()
    {
        cursor = GetComponent<PlayerCursor>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cursor.CurrentHoveredObject != null && !BuildingManager.Instance.IsBuildMode )
        {
                if (cursor.CurrentHoveredObject is Resource && GetComponent<PlayerTurnManager>().MovesRemaining > 0)
                {
                    OnGather?.Invoke();
                    cursor.CurrentHoveredObject.OnClick();
                    return;
                }
                cursor.CurrentHoveredObject.OnClick();
        }
    }
}