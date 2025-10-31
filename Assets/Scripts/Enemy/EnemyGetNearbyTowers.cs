using System;
using UnityEngine;

public class EnemyGetNearbyTowers : MonoBehaviour, IGetNearbyTowers
{
    [SerializeField] private LayerMask towerLayer;
    public TowerDataHolder[] GetNearbyTowers(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, towerLayer);
        Array.Sort(colliders, (a, b) => Vector2.Distance(transform.position, a.transform.position)
            .CompareTo(Vector2.Distance(transform.position, b.transform.position)));
        TowerDataHolder[] towers = new TowerDataHolder[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            towers[i] = colliders[i].GetComponent<TowerDataHolder>();
        }
        return towers;
    }
}