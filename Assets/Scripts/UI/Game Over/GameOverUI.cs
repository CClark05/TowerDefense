using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI highestWave, enemiesKilled, maxDPS, coinsEarned;
    [SerializeField] private Button_Base menuButton, replayButton;
    [SerializeField] private TextMeshProUGUI headerText;
    private void Start()
    {
        PlayerLife.Instance.OnGameOver += OnGameOver;
        EnemyManager.Instance.OnNoWavesLeft += OnVictory;
        menuButton.OnClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneLoader.Scenes.MainMenu.ToString());
        });
        replayButton.OnClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        background.SetActive(false);
    }

    private void OnVictory(GameOverData data)
    {
        OnGameOver(data);
        headerText.text = "<color=#DE9E41>VICTORY</color>";
    }

    private void OnGameOver(GameOverData data)
    {
        background.SetActive(true);
        highestWave.text = data.HighestWave.ToString();
        enemiesKilled.text = data.EnemiesKilled.ToString();
        maxDPS.text = Math.Round(data.MaxDps, MidpointRounding.AwayFromZero).ToString();
        coinsEarned.text = data.CoinsEarned.ToString();
    }
}
