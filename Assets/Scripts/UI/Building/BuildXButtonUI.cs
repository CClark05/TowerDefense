using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildXButtonUI : MonoBehaviour
{
    [SerializeField] private Button_Hover button;
    [SerializeField] private Sprite whiteOutlineSprite;
    private Image buttonImage;
    private Sprite originalSprite;
    private Vector3 originalScale;
    private void Awake()
    {
        originalScale = transform.localScale;
        button.gameObject.SetActive(false);
        buttonImage = button.GetComponent<Image>();
        originalSprite = buttonImage.sprite;
    }

    private void Start()
    {
        button.OnHover += () =>
        {
            GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
            BuildingManager.Instance.ToggleBuildPreview(false);
            buttonImage.sprite = whiteOutlineSprite;
            transform.DOKill();
            transform.localRotation = Quaternion.identity;
            transform.DOScale(originalScale * 1.2f, 0.25f).SetEase(Ease.OutBack);
            transform.DOPunchRotation(new Vector3(0, 0, 2.5f), 0.25f, 6, 0.5f);
        };
        button.OnLeaveHover += () =>
        {
            GameManager.Instance.SetCursor(GameManager.Cursors.Default);
            BuildingManager.Instance.ToggleBuildPreview(true);
            buttonImage.sprite = originalSprite;
            transform.DOKill();
            transform.localRotation = Quaternion.identity;
            transform.DOScale(originalScale, 0.25f).SetEase(Ease.OutSine);
        };
        button.OnClick.AddListener(() =>
        {
            BuildingManager.Instance.ExitBuildMode();
        });
        BuildingManager.Instance.OnEnterBuildMode += () =>
        {
            button.gameObject.SetActive(true);
        };
        BuildingManager.Instance.OnExitBuildMode += () =>
        {
            button.gameObject.SetActive(false);
        };
    }
}
