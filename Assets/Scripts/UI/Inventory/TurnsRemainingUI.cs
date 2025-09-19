using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TurnsRemainingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    private void Start()
    {
        counterText.text = "Turns : " + PlayerTurnManager.Instance.MovesRemaining.ToString();
        
        PlayerTurnManager.Instance.OnMovesUpdated += moves =>
        {
            counterText.text = "Turns : " + moves.ToString();
        };
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            counterText.gameObject.SetActive(false);
        };
        EnemyManager.Instance.OnIdle += () =>
        {
            counterText.gameObject.SetActive(true);
        };
    }
}
