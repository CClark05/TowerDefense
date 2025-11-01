using System;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncounterSettings settings;
    [SerializeField] private GameObject visual;
    public SOEvent OnShowVisual;
    private void Start()
    {
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            int currentWave = EnemyManager.Instance.CurrentWave;
            if (currentWave >= settings.firstWave && (currentWave - settings.firstWave) % settings.showEveryXWaves == 0)
            {
                OnShowVisual.Raise(this);
            }
        };
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            visual.SetActive(false);
        };
    }
}
