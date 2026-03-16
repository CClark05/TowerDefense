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
    }

    private void Start()
    {
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
    }
}
