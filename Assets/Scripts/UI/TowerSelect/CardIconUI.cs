using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardIconUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image selectionOutline;
    [SerializeField] private Button_Scale_Hover button;
    [SerializeField] private Image redOutline;
    public Button_Scale_Hover Button { get; private set; }
    public SkillData SkillData { get; private set; }
    public IUsesCards usesCards { get; private set; }
    private bool selected;
    private SkillContext skillContext;
    public bool Selected
    {
        get => selected;
        set
        {
            if (selected == value) return;
            selected = value;
            UpdateVisual();
        }
    }

    public void Init(SkillData skillData, IUsesCards usesCards)
    {
        Button = button;
        SkillData = skillData;
        this.usesCards = usesCards;
        image.sprite = skillData.icon;
        skillContext = GetComponentInParent<TowerDataHolder>().SkillContext;
        skillContext.OnCardPlayTwiceUpdated += OnCardPlayTwiceUpdated;
        redOutline.gameObject.SetActive(skillContext.ActiveSkills.LastOrDefault(s => s.Data == skillData).PlayTwice);
        if (ColorUtility.TryParseHtmlString("#75a743", out Color greenColor))
            GetComponent<CardIconDragDrop>().OnIsOverReceiverUpdated += isOver =>
            {
                if (isOver)
                {
                    selectionOutline.color = greenColor;
                    return;
                }

                selectionOutline.color = Color.white;
            };
    }
    private void OnCardPlayTwiceUpdated(SkillData data, bool playTwice)
    {
        if (data != SkillData) return;
        redOutline.gameObject.SetActive(playTwice);
    }
    private void UpdateVisual()
    {
        Button.ToggleScaleAnimation(selected);
        if (!selected) Button.ScaleBackToNormal();
        selectionOutline.gameObject.SetActive(selected);
    }

    private void OnDestroy()
    {
        skillContext.OnCardPlayTwiceUpdated -= OnCardPlayTwiceUpdated;
    }
}