using System;
using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI livesText;

    private void Start()
    {
        PlayerLife.Instance.OnLivesUpdated += OnLivesUpdated;
        livesText .text = PlayerLife.Instance.CurrentLives.ToString();
    }

    private void OnLivesUpdated(int lives)
    {
        livesText.text = lives.ToString();
    }
    
}
