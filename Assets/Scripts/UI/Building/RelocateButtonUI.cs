using System;
using UnityEngine;
using UnityEngine.UI;

public class RelocateButtonUI : MonoBehaviour
{
    [SerializeField] private Button_Hover button;
    [SerializeField] private Sprite whiteOutlineSprite;
    private Sprite originalSprite;
    private Image buttonImage;
    private TowerDataHolder selectedTower;
    private void Start()
    {
        buttonImage = button.GetComponent<Image>();
        originalSprite = buttonImage.sprite;
        button.gameObject.SetActive(false);
        button.OnHover += () =>
        {
            GameManager.Instance.SetCursor(GameManager.Cursors.OpenHand);
            buttonImage.sprite = whiteOutlineSprite;
        };
        button.OnLeaveHover += () =>
        {
            GameManager.Instance.SetCursor(GameManager.Cursors.Default);
            buttonImage.sprite = originalSprite;
        };
        button.OnClick.AddListener(() =>
        {
            //StartCoroutine(BuildingManager.Instance.MoveBuild(selectedTower.gameObject));
        });
    }
}
