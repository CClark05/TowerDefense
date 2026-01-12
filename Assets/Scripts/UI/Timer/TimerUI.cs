using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private float elapsed;
    private bool paused = true;
    private void Start()
    {
        timerText.text = "";
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            paused = false;
        };
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            paused = true;
        };
    }
    private void Update()
    {
        if (paused) return;
        elapsed += Time.deltaTime;
        UpdateText(elapsed);
    }

    private void UpdateText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;

        if (minutes > 0)
            timerText.text = $"{minutes}:{seconds:00.00}";
        else
            timerText.text = seconds.ToString("0.00");
        
    }
}
