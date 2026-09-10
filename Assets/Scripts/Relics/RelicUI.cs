using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicUI : MonoBehaviour
{
    private Button_Hover button;
    private Image icon;
    private RelicInstance instance;
    public event Action OnClick;
    private Vector3 originalScale;
    [SerializeField] private GameObject descriptionTab;
    private void Awake()
    {
        icon = GetComponent<Image>();
        button = GetComponent<Button_Hover>();
        button.enabled = false;
    }

    private void Start()
    {
        descriptionTab.GetComponent<Canvas>().sortingLayerName = "Screen UI";
        button.OnClick.AddListener(() =>
        {
            OnClick?.Invoke();
            StartCoroutine(RelicManager.Instance.AddRelic(this));
        });
        button.OnHover += () =>
        {
            originalScale = transform.localScale;
            transform.DOScale(originalScale * 1.05f, 0.05f);
            descriptionTab.SetActive(true);
        };
        button.OnLeaveHover += () =>
        {
            transform.DOScale(originalScale, 0.05f);
            descriptionTab.SetActive(false);
        };
    }
    public void Init(RelicInstance instance)
    {
        icon.sprite = instance.Data.sprite;
        descriptionTab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = instance.Data.name;
        descriptionTab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = instance.Data.description;
        instance.OnUsed += () =>
        {
            GetComponent<SquishAnimation>().Squish(0.5f);
        };
        transform.localScale = Vector3.zero;
        transform.DOLocalMove(Vector3.zero, 0.35f);
        transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            GetComponent<SquishAnimation>().Squish(0.5f);
            button.enabled = true;
        });
    }
}
