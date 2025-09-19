using System;
using TMPro;
using UnityEngine;

public class CardSlotUI : MonoBehaviour
{
    [SerializeField] private GameObject fullCardPrefab;
    private Transform toolTipLayer;
    private SkillCardUI fullCard;
    public SkillData SkillData { get; private set; }
    private bool selected;
    private bool disableButton;
    public event Action OnSellCard;
    public void Init(SkillData data)
    {
        toolTipLayer = ToolTipLayer.Instance.transform;
        SkillData = data;
        GetComponentInChildren<TextMeshProUGUI>().text = data.name;
        var button = GetComponent<Button_Hover>();
        button.OnHover += () =>
        {
            if (disableButton) return;
            if (fullCard == null)
            {
                fullCard = Instantiate(fullCardPrefab, toolTipLayer).GetComponent<SkillCardUI>();
                fullCard.transform.SetAsLastSibling();
            }

            fullCard.gameObject.SetActive(true);
            fullCard.GetComponent<SetCardData>().SetData(SkillData);
            fullCard.transform.position = (Vector2)transform.position + new Vector2(0, -50f);
            fullCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
        };

        button.OnLeaveHover += () =>
        {
            if (fullCard != null && !selected)
            {
                fullCard.gameObject.SetActive(false);
            }

            selected = false;
            disableButton = false;
        };

        button.OnClick.AddListener(() =>
        {
            selected = !selected;
            fullCard.GetComponent<CanvasGroup>().enabled = !selected;
            fullCard.ToggleSellButton(selected);
            fullCard.ToggleButtonOnClick(!selected);
            button.enabled = !selected;
            fullCard.OnDisableButton += () =>
            {
                disableButton = true;
                button.enabled = true;
            };
            fullCard.OnSellCard += () =>
            {
                OnSellCard?.Invoke();
                Destroy(gameObject);
            };
        });
    }

    private void OnDisable()
    {
        if (fullCard != null)
            Destroy(fullCard.gameObject);
    }
}