using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonUI : Singleton<PlayButtonUI>
{
    [SerializeField] private Button_Base playButton;
    [SerializeField] private Button_Base fastForwardButton;
    private Color originalFastForwardColor;
    public event Action OnNextWave;
    private void Start()
    {
        originalFastForwardColor = fastForwardButton.GetComponent<Image>().color;
        fastForwardButton.gameObject.SetActive(false);
        playButton.OnClick.AddListener(() =>
        {
            OnNextWave?.Invoke();
            playButton.gameObject.SetActive(false);
            fastForwardButton.gameObject.SetActive(true);
        });
        fastForwardButton.OnClick.AddListener(() =>
        {
            bool normalSpeed = Mathf.Approximately(Time.timeScale, 1);
            Time.timeScale = normalSpeed ? 1.5f : 1;
            Array.ForEach(fastForwardButton.GetComponentsInChildren<Image>(true),
                img => img.color = normalSpeed ? Color.white : originalFastForwardColor);
        });
        EnemyManager.Instance.OnWaveComplete += () =>
        {
            Array.ForEach(fastForwardButton.GetComponentsInChildren<Image>(true),
                img => img.color = originalFastForwardColor);
        };
    }

    private void OnEnable()
    {
        EnemyManager.Instance.OnIdle += EnableButton;
        CardSelectUI.Instance.OnSelectedCard += EnableButton;
    }

    private void EnableButton()
    {
        playButton.gameObject.SetActive(true);
        fastForwardButton.gameObject.SetActive(false);
    }

}