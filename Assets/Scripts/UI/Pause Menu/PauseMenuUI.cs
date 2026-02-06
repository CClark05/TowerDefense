
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button_Base resumeButton, restartButton, quitButton;
    [SerializeField] private GameObject background;
    private float originalTimeScale;
    private void Start()
    {
        resumeButton.OnClick.AddListener(() =>
        {
            Time.timeScale = originalTimeScale;
            background.SetActive(false);
        });
        restartButton.OnClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        quitButton.OnClick.AddListener(Application.Quit);
        background.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !BuildingManager.Instance.IsInBuildMode)
        {
            if (background.activeSelf)
            {
                Time.timeScale = originalTimeScale;
                background.SetActive(false);
            }
            else
            {
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                background.SetActive(true);
            }
        }
    }
}
