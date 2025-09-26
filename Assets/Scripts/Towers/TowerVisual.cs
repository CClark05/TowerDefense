using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class TowerVisual : MonoBehaviour
{
    [SerializeField] private GameObject rangeVisual;
    private bool isSelected;
    private TowerDataHolder dataHolder;
    private EnemyManager enemyManager;
    private void Start()
    {
        TowerHoverable.OnClickTowerStatic += OnClickTower;
        TowerHoverable.OnHoverTowerStatic += OnHover;
        TowerHoverable.OnLeaveTowerHoverStatic += OnLeaveHover;
        enemyManager = EnemyManager.Instance;
        enemyManager.OnWaveStarted += DisableSelection;
        enemyManager.OnWaveComplete += OnWaveComplete;
        dataHolder = GetComponent<TowerDataHolder>();
        dataHolder.RuntimeData.OnRangeUpdated += range =>
        {
            rangeVisual.transform.localScale = new Vector3(range * 2f, range * 2f, 1);
        };
        rangeVisual.transform.localScale = new Vector3(dataHolder.Data.range * 2f, dataHolder.Data.range * 2f, 1);
        rangeVisual.gameObject.SetActive(false);
    }

    private void OnWaveComplete(int obj, bool _)
    {
        isSelected = false;
        rangeVisual.SetActive(false);
    }

    private void DisableSelection()
    {
        isSelected = false;
        rangeVisual.SetActive(false);
    }
    private void OnClickTower(TowerDataHolder data)
    {
        if (data == GetComponent<TowerDataHolder>())
        {
            isSelected = !isSelected;
            rangeVisual.SetActive(isSelected);
        }
    }

    private void OnHover(TowerDataHolder data)
    {
        if (data == GetComponent<TowerDataHolder>())
            rangeVisual.SetActive(true);
    }

    private void OnLeaveHover()
    {
        if (isSelected) return;
        rangeVisual.SetActive(false);
    }

    private void OnDisable()
    {
        TowerHoverable.OnClickTowerStatic -= OnClickTower;
        TowerHoverable.OnHoverTowerStatic -= OnHover;
        TowerHoverable.OnLeaveTowerHoverStatic -= OnLeaveHover;
        enemyManager.OnWaveStarted -= DisableSelection;
        enemyManager.OnWaveComplete -= OnWaveComplete;
    }
}
