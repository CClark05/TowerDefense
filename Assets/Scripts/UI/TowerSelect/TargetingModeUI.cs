using System;
using TMPro;
using UnityEngine;

public class TargetingModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TowerShooting towerShooting;
    private Button_Base button;
    public event Action<TargetingModes> OnTargetingModeUpdated;
    private void Awake()
    {
        button = GetComponent<Button_Base>();
        button.OnClick.AddListener(() =>
        {
            int currentMode = (int)towerShooting.TargetingMode;
            int next = (currentMode + 1) % System.Enum.GetValues(typeof(TargetingModes)).Length;
            OnTargetingModeUpdated?.Invoke((TargetingModes)next);
        });
        
    }
    private void Start()
    {
        OnTargetingModeUpdated += mode =>
        {
            text.text = mode.ToString();
        };
        text.text = towerShooting.TargetingMode.ToString();
    }
}
