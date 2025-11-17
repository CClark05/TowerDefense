using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class TowerVisual : MonoBehaviour
{
    [SerializeField] private GameObject rangeVisual, crossbowVisual;
    private bool isSelected;
    private TowerDataHolder dataHolder;
    private EnemyManager enemyManager;
    private CardSelectUI cardSelectUI;
    private TowerShooting towerShooting;
    private void Start()
    {
        TowerHoverable.OnClickTowerStatic += OnClickTower;
        TowerHoverable.OnHoverTowerStatic += OnHover;
        TowerHoverable.OnLeaveTowerHoverStatic += OnLeaveHover;
        enemyManager = EnemyManager.Instance;
        cardSelectUI = CardSelectUI.Instance;
        enemyManager.OnWaveStarted += DisableSelection;
        enemyManager.OnWaveComplete += DisableSelection;
        cardSelectUI.OnShowCards += DisableSelection;
        dataHolder = GetComponent<TowerDataHolder>();
        towerShooting = GetComponent<TowerShooting>();
        dataHolder.RuntimeData.OnRangeUpdated += range =>
        {
            rangeVisual.transform.localScale = new Vector3(range * 2f, range * 2f, 1);
        };
        rangeVisual.transform.localScale = new Vector3(dataHolder.Data.range * 2f, dataHolder.Data.range * 2f, 1);
        rangeVisual.gameObject.SetActive(false);
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

    private void Update()
    {
        if (!towerShooting.Direction.HasValue) return;
        Vector2 dir = towerShooting.Direction.Value;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = crossbowVisual.transform.rotation.eulerAngles.z;
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));

        if (angleDifference > 0.5f) 
        {
            float speed = 25f;
            crossbowVisual.transform.rotation = Quaternion.Lerp(crossbowVisual.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * speed);
        }
    }

    private void OnDisable()
    {
        TowerHoverable.OnClickTowerStatic -= OnClickTower;
        TowerHoverable.OnHoverTowerStatic -= OnHover;
        TowerHoverable.OnLeaveTowerHoverStatic -= OnLeaveHover;
        enemyManager.OnWaveStarted -= DisableSelection;
        enemyManager.OnWaveComplete -= DisableSelection;
        cardSelectUI.OnShowCards -= DisableSelection;
    }
}
