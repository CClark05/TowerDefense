using System;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EncounterSettings settings;
    [SerializeField] private GameObject visual;
    public SOEvent OnShowVisual;
    private bool visualActive;

    private void Start()
    {
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            var encounter = EncounterGenerator.Instance.GetEncounter(EnemyManager.Instance.CurrentWave - 1);
            if (encounter == settings)
            {
                OnShowVisual.Raise(this);
                visualActive = true;
            }
        };
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            if (!visualActive) return;
            visual.GetComponent<GridObjectPlacement>().DriveAway();
            visualActive = false;
        };
    }
}