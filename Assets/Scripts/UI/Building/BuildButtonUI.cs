using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BuildButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button_Base button;
    [SerializeField] private TowerData towerData;
    private TowerData runtimeTowerData;
    public TowerData RuntimeTowerData => runtimeTowerData;
    [SerializeField] private GameObject background;
    private Color originalTextColor;
    public event Action<TowerData> OnBuildMode;
    public event Action OnExitBuildMode;
    public static event Action OnNotEnoughCoins;
    private void Start()
    {
        runtimeTowerData = towerData.CloneRuntime();
        originalTextColor = costText.color;
        costText.text = towerData.cost.ToString();
        button.OnClick.AddListener(TryEnterBuildMode);
        costText.color = PlayerInventory.Instance.Coins >= towerData.cost ? originalTextColor : ColorPicker.red;
        PlayerInventory.Instance.OnCoinsUpdated += coins => costText.color = coins >= runtimeTowerData.Cost ? originalTextColor : ColorPicker.red;
        EnemyManager.Instance.OnWaveStarted += () => background.SetActive(false);
        EnemyManager.Instance.OnWaveComplete += () => background.SetActive(true);
        BuildingManager.Instance.OnPlacedBuild += data => background.SetActive(true);
        runtimeTowerData.OnCostIncreased += cost =>
        {
            costText.text = cost.ToString();
            costText.color = PlayerInventory.Instance.Coins >= runtimeTowerData.Cost ? originalTextColor : ColorPicker.red;
        };

    }
    private Coroutine buildDelayRoutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (background.activeSelf)
            {
                TryEnterBuildMode();
                return;
            }
            ExitBuildMode();
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !background.activeSelf)
            ExitBuildMode();
    }
    private void TryEnterBuildMode()
    {
        if (PlayerInventory.Instance.Coins < runtimeTowerData.cost)
        {
            OnNotEnoughCoins?.Invoke();
            return;
        }
        OnBuildMode?.Invoke(runtimeTowerData);
        background.SetActive(false);
    }

    private void ExitBuildMode()
    {
        OnExitBuildMode?.Invoke();
        background.SetActive(true);
    }
}

