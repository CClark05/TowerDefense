using System;
using TMPro;
using UnityEngine;

public class WaveCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveCountText;

    private void Start()
    {
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            waveCountText.text = EnemyManager.Instance.CurrentWave.ToString();
        };
    }
}