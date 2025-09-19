using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DamageMarkerManager : Singleton<DamageMarkerManager>
{
    [SerializeField] private GameObject hitMarkerPrefab;
    [SerializeField] private Canvas canvas;
    private Dictionary<GameObject, Vector2> currentMarkers = new();
    private void Start()
    {
        TowerShooting.OnDealDamageStatic += OnDealDamage;
    }

    private void OnDealDamage(Vector2 position, DamageData damageData)
    {
        const float minDistance = 0.5f;
        const int maxTries = 10;
        int attempts = 0;
        Vector2 randomPosition;
        Vector2 endPosition;
        do
        {
            randomPosition = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.8f, 1.2f));
            endPosition = position + randomPosition;

            bool overlaps = false;
            foreach (var marker in currentMarkers)
            {
                if (Vector2.Distance(marker.Value, endPosition) < minDistance)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps) break;

            attempts++;
        } while (attempts < maxTries);
        
        var newMarker = Instantiate(hitMarkerPrefab, endPosition, Quaternion.identity);
        newMarker.transform.parent = canvas.transform;
        newMarker.GetComponent<DamageMarker>().Init(damageData.finalDamage, damageData.colors.ToArray(), damageData.damageMarkerSizeMult, damageData.damageMarkerPunchEffect, randomPosition);
        currentMarkers.Add(newMarker, endPosition);
        newMarker.GetComponent<DamageMarkerAnimation>().OnDie += () => currentMarkers.Remove(newMarker);
    }

    private void OnDisable()
    {
        TowerShooting.OnDealDamageStatic -= OnDealDamage;
    }
}