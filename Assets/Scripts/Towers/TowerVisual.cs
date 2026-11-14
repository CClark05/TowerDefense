using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class TowerVisual : MonoBehaviour
{
    [SerializeField] private GameObject rangeVisual;

    [FormerlySerializedAs("crossbowVisual")] [SerializeField]
    private GameObject visual;

    [SerializeField] private GameObject baseVisual;
    [SerializeField] private bool rotateTower = true;
    [SerializeField] private WhiteOutlineEffect outlineEffect;
    private bool isSelected;
    private TowerDataHolder dataHolder;
    private EnemyManager enemyManager;
    private CardSelectUI cardSelectUI;
    private TowerShooting towerShooting;
    private TowerHoverable towerHoverable;
    private TowerDrag towerDrag;
    private void Start()
    {
        towerHoverable = GetComponent<TowerHoverable>();
        towerDrag = GetComponent<TowerDrag>();
        towerHoverable.OnHoverTower += OnHover;
        towerHoverable.OnLeaveHoverTower += OnLeaveHover;
        enemyManager = EnemyManager.Instance;
        cardSelectUI = CardSelectUI.Instance;
        enemyManager.OnWaveStarted += DisableSelection;
        enemyManager.OnWaveComplete += DisableSelection;
        cardSelectUI.OnShowCards += DisableSelection;
        dataHolder = GetComponent<TowerDataHolder>();
        towerShooting = GetComponent<TowerShooting>();
        dataHolder.RuntimeData.OnRangeUpdated += range => { rangeVisual.transform.localScale = new Vector3(range * 2f, range * 2f, 1); };
        rangeVisual.transform.localScale = new Vector3(dataHolder.Data.range * 2f, dataHolder.Data.range * 2f, 1);
        rangeVisual.transform.GetChild(0).gameObject.SetActive(false);
        towerShooting.OnToggleShooting += disable =>
        {
            visual.SetActive(!disable);
            baseVisual.SetActive(!disable);
        };
        towerDrag.OnDoneMoving += (successful) =>
        {
            if (successful) return;
            //GameManager.Instance.SetCursor(GameManager.Cursors.Default);
            rangeVisual.transform.GetChild(0).gameObject.SetActive(false);
            outlineEffect.ToggleWhiteOutline(false);
        };
        towerDrag.OnSelect += () =>
        {
            outlineEffect.ToggleWhiteOutline(true);
            visual.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        };
        towerDrag.OnDeselect += () =>
        {
            outlineEffect.ToggleWhiteOutline(false);
            rangeVisual.transform.GetChild(0).gameObject.SetActive(false);
            visual.transform.localScale = new Vector3(1f, 1f, 1f);
        };
    }


    private void DisableSelection()
    {
        outlineEffect.ToggleWhiteOutline(false);
        isSelected = false;
        rangeVisual.transform.GetChild(0).gameObject.SetActive(false);
    }


    private void OnHover()
    {
        if (TowerDrag.IsAnyMoving) return;
        GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
        rangeVisual.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnLeaveHover()
    {
        if (TowerDrag.IsAnyMoving) return;
        GameManager.Instance.SetCursor(GameManager.Cursors.Default);
        if(towerDrag.IsSelected) return;
        rangeVisual.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!rotateTower) return;
        if (!towerShooting.Direction.HasValue) return;
        Vector2 dir = towerShooting.Direction.Value;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = visual.transform.rotation.eulerAngles.z;
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));

        if (angleDifference > 0.5f)
        {
            float speed = 25f;
            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * speed);
        }
    }

    public void ToggleTowerVisibility(bool enabled)
    {
        baseVisual.SetActive(enabled);
    }

    private void OnDisable()
    {
        enemyManager.OnWaveStarted -= DisableSelection;
        enemyManager.OnWaveComplete -= DisableSelection;
        cardSelectUI.OnShowCards -= DisableSelection;
    }
}