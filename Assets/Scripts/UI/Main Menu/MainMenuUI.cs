using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button_Base playButton;

    private void Start()
    {
        playButton.OnClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneLoader.Scenes.Game.ToString());
        });
    }
}