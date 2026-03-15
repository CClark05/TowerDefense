using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetCardData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText, damageText, coinsText;
    private string originalDescription;
    [SerializeField] private GameObject sideTabPrefab;
    [SerializeField] private TextMeshProUGUI runtimeStatText;
    [SerializeField] private SkillData skillData;
    [SerializeField] private Image skillIconImage;
    [SerializeField] private VerticalLayoutGroup tabGroup;
    [SerializeField] private Image laminatedImage;
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
        laminatedImage.gameObject.SetActive(false);
        if (skillData.statusEffects.Any(e => e.data is LaminatedEffectData))
            laminatedImage.gameObject.SetActive(true);
        
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
        if(damageText != null)
            damageText.gameObject.SetActive(false);
        UpdateVisual();
    }

    public void SetInstance(SkillInstance instance)
    {
        SkillInstance = instance;
        if (SkillInstance.RuntimeStat.HasValue)
            runtimeStatText.text = $"({SkillInstance.Data.FormatRuntimeStat(SkillInstance.RuntimeStat.Value)})";
        UpdateDescription(instance.PlayCount);
        if (instance.Damage > 0 && damageText != null)
        {
            damageText.gameObject.SetActive(true);
            damageText.text = "Damage:";
            damageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = instance.Damage.ToString();
        }
        if(instance.CoinsGenerated > 0 && coinsText != null)
        {
            coinsText.gameObject.SetActive(true);
            coinsText.text = $"${instance.CoinsGenerated}";
        }
        SkillInstance.OnRuntimeStatUpdated += value => { runtimeStatText.text = $"({SkillInstance.Data.FormatRuntimeStat(value)})"; };
        SkillInstance.OnPlayCountUpdated += UpdateDescription;
        SkillInstance.OnIsLaminatedUpdated += laminated =>
        {
            laminatedImage.gameObject.SetActive(laminated);
        };
        void UpdateDescription(int playCount)
        {
            descriptionText.text = Regex.Replace(
                originalDescription,
                @"\+(\$?)(\d+(?:\.\d+)?)",
                match =>
                {
                    string currency = match.Groups[1].Value; 
                    float baseValue = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    float newValue = baseValue * playCount;

                    return $"+{currency}{newValue:0.##}";
                },
                RegexOptions.CultureInvariant
            );
        }

        SkillInstance.OnDamageUpdated += (damage) =>
        {
            if (damageText == null) return;
            damageText.gameObject.SetActive(true);
            damageText.text = "Damage:";
            damageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{damage}";
        };
        SkillInstance.OnCoinsUpdated += (coins) =>
        {
            if (coinsText == null) return;
            coinsText.gameObject.SetActive(true);
            coinsText.text = $"${coins}";
        };
    }
    
    public void DisableTabs()
    {
        foreach (var tab in currentTabs)
            tab.SetActive(false);
    }
}