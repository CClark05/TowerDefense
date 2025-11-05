using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardIconUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image selectionOutline;
    [SerializeField] private Image disabledOverlay;
    [SerializeField] private Button_Scale_Hover button;
    [SerializeField] private Image redOutline;
    [SerializeField] private TextMeshProUGUI playCountText;
    public Button_Scale_Hover Button { get; private set; }
    public SkillInstance SkillInstance { get; private set; }
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

    public void Init(SkillInstance instance, IUsesCards usesCards)
    {
        Button = button;
        SkillInstance = instance;
        this.usesCards = usesCards;
        disabledOverlay.enabled = instance.IsDisabled;
        image.sprite = instance.Data.icon;
        skillContext = GetComponentInParent<TowerDataHolder>().SkillContext;
        SkillInstance.OnPlayCountUpdated += CardPlayCountUpdated;
        SkillInstance.OnIsDisabledUpdated += OnIsDisabledUpdated;
        redOutline.gameObject.SetActive(instance.PlayCount > 1);
        playCountText.text = instance.PlayCount > 1 ? instance.PlayCount.ToString() : "";
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

    private void OnIsDisabledUpdated(bool disabled)
    {
        disabledOverlay.enabled = disabled;
    }

    private void CardPlayCountUpdated(int playCount)
    {
        if(redOutline == null) return;
        redOutline.gameObject.SetActive(playCount > 1);
        playCountText.text = playCount > 1 ? playCount.ToString() : "";
    }
    private void UpdateVisual()
    {
        Button.ToggleScaleAnimation(selected);
        if (!selected) Button.ScaleBackToNormal();
        selectionOutline.gameObject.SetActive(selected);
    }

    private void OnDestroy()
    {
        SkillInstance.OnPlayCountUpdated -= CardPlayCountUpdated;
        SkillInstance.OnIsDisabledUpdated -= OnIsDisabledUpdated;
    }
}