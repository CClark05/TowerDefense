using System;
using UnityEngine;
using UnityEngine.UI;


public class BuildingUI : Singleton<BuildingUI>
{
    [SerializeField] private Button_Base[] buildButtons;
    [SerializeField] private Button_Base buildModeButton;
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private Button_Base XButton;
    [SerializeField] private Button_Hover trashButton;
    public event Action<BuildableObjectData> OnButtonPress;
    public event Action OnEnterBuildMode;
    public event Action OnExitBuildMode;
    public event Action OnTrash;
    private void Start()
    {
        foreach (var button in buildButtons)
        {
            button.OnClick.AddListener(() =>
            {
                OnButtonPress?.Invoke(button.GetComponent<BuildButtonUI>().Data);
                trashButton.gameObject.SetActive(true);
                XButton.gameObject.SetActive(false);
                GameObject buildVisual = null;
                trashButton.OnHover += () =>
                {
                    buildVisual = new GameObject("Build Visual");
                    buildVisual.AddComponent<Image>();
                    buildVisual.transform.SetParent(trashButton.transform);
                    Sprite sprite = button.GetComponent<BuildButtonUI>().Data.prefab.GetComponent<SpriteRenderer>().sprite;
                    buildVisual.GetComponent<Image>().sprite = sprite;
                    buildVisual.GetComponent<RectTransform>().localScale = Vector3.one;
                    buildVisual.GetComponent<RectTransform>().sizeDelta = sprite.rect.size * 2f;
                    buildVisual.GetComponent<Image>().raycastTarget = false;
                    buildVisual.AddComponent<FollowMouseUI>();
                };
                trashButton.OnLeaveHover += () =>
                {
                    foreach (Transform child in trashButton.transform)
                    {
                        Destroy(child.gameObject);
                    }
                };
                trashButton.OnClick.AddListener(() =>
                {
                    foreach (Transform child in trashButton.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    CancelBuild();
                    OnTrash?.Invoke();
                });
            });
        }

        BuildingManager.Instance.OnEnterBuildMode += EnterBuildMode;
        BuildingManager.Instance.OnExitBuildMode += ExitBuildMode;
        BuildingManager.Instance.OnCancelBuild += CancelBuild;

        void CancelBuild()
        {
            trashButton.gameObject.SetActive(false);
            XButton.gameObject.SetActive(true);
        }
        buildModeButton.OnClick.AddListener(EnterBuildMode);
        XButton.OnClick.AddListener(ExitBuildMode);
        EnemyManager.Instance.OnWaveStarted += () =>
        {
            buildModeButton.gameObject.SetActive(false);
        };
        EnemyManager.Instance.OnIdle += () =>
        {
            buildModeButton.gameObject.SetActive(true);
        };
    }

    private void EnterBuildMode()
    {
        buildMenu.SetActive(true);
        trashButton.gameObject.SetActive(false);
        OnEnterBuildMode?.Invoke();
    }

    private void ExitBuildMode()
    {
        trashButton.gameObject.SetActive(false);
        XButton.gameObject.SetActive(true);

        buildMenu.SetActive(false);
        OnExitBuildMode?.Invoke();
    }
}