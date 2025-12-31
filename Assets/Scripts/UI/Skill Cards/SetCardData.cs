using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;

public class SetCardData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText;
    private string originalDescription;
    [SerializeField] private GameObject sideTabPrefab;
    [SerializeField] private TextMeshProUGUI runtimeStatText;
    [SerializeField] private SkillData skillData;
    [SerializeField] private Image skillIconImage;
    [SerializeField] private VerticalLayoutGroup tabGroup;
    public SkillData SkillData => skillData;
    public SkillInstance SkillInstance { get; private set; }
    private List<GameObject> currentTabs = new();
    public InterfaceReference<ICardUI> cardUI;

    private void Start()
    {
        if (skillData.icon != null)
            skillIconImage.sprite = skillData.icon;
        cardUI.Value.OnHoverCard += () =>
        {
            foreach (var tab in currentTabs)
                tab.SetActive(true);
        };
        cardUI.Value.OnLeaveHoverCard += () =>
        {
            foreach (var tab in currentTabs)
                tab.SetActive(false);
        };
    }

    private void UpdateVisual()
    {
        nameText.text = skillData.name;
        descriptionText.text = skillData.description;
        originalDescription = skillData.description;
        foreach (var effect in skillData.statusEffects)
        {
            if (!effect.showInUI) continue;
            AddTab(effect);
        }

        foreach (var buff in skillData.buffs)
        {
            if (!buff.showInUI) continue;
            AddTab(buff);
        }

        void AddTab(EffectEntry effect)
        {
            var tab = Instantiate(sideTabPrefab, tabGroup.transform);
            var name = tab.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            name.text = effect.data.name;
            name.color = effect.data.color;
            tab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = effect.data.description;
            currentTabs.Add(tab);
            tab.SetActive(false);
        }
    }

    public void SetData(SkillData data)
    {
        skillData = data;
        UpdateVisual();
    }

    public void SetInstance(SkillInstance instance)
    {
        SkillInstance = instance;
        if (SkillInstance.RuntimeStat.HasValue)
            runtimeStatText.text = $"({SkillInstance.Data.FormatRuntimeStat(SkillInstance.RuntimeStat.Value)})";
        UpdateDescription(instance.PlayCount);
        SkillInstance.OnRuntimeStatUpdated += value => { runtimeStatText.text = $"({SkillInstance.Data.FormatRuntimeStat(value)})"; };
        SkillInstance.OnPlayCountUpdated += UpdateDescription;

        void UpdateDescription(int playCount)
        {
            descriptionText.text = Regex.Replace(
                originalDescription,
                @"\+(\d+(?:\.\d+)?)",
                match =>
                {
                    float baseValue = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    float newValue = baseValue * playCount;
                    return $"+{newValue:0.##}";
                },
                RegexOptions.CultureInvariant
            );
        }
    }

    public void DisableTabs()
    {
        foreach (var tab in currentTabs)
            tab.SetActive(false);
    }
}