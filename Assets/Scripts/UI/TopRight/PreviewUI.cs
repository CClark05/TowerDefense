using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewUI : MonoBehaviour
{
    [SerializeField] private Sprite emptyIcon;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private EncounterSettings shop;
    private EncounterGenerator encounterGenerator;
    private void Start()
    {
        encounterGenerator = EncounterGenerator.Instance;
        ShowNextWaves(1);
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            ShowNextWaves(EnemyManager.Instance.CurrentWave);
        };
    }
    

    private void ShowNextWaves(int currentWave)
    {
        for (int i = currentWave; i < currentWave + layoutGroup.transform.childCount; i++)
        {
            var encounter = encounterGenerator.GetEncounter(i);
            var icon = encounter?.icon;
            layoutGroup.transform.GetChild(i - currentWave).GetComponent<Image>().sprite = icon == null || encounter == shop ? emptyIcon : icon;
        }
    }
}
