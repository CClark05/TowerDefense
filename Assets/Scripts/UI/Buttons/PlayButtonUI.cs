using System;
using UnityEngine;

public class PlayButtonUI : Singleton<PlayButtonUI>
{
    [SerializeField] private Button_Base playButton;
    public event Action OnNextWave;
    private void Start()
    {
        playButton.OnClick.AddListener(() =>
        {
            OnNextWave?.Invoke();
            playButton.gameObject.SetActive(false);
        });
    }

    private void OnEnable()
    {
        EnemyManager.Instance.OnIdle += EnableButton;
        CardSelectUI.Instance.OnSelectedCard += EnableButton;
    }

    private void EnableButton()
    {
        playButton.gameObject.SetActive(true);
    }

}
