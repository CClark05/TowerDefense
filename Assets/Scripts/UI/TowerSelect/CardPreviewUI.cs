using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText, runtimeStatText, damageText, coinText;
    [SerializeField] private VerticalLayoutGroup tabLayout;
    [SerializeField] private GameObject tabPrefab;
    private List<GameObject> activeTabs = new();
    private SkillInstance currentInstance;

    public void SetSkill(SkillInstance instance)
    {
        if (currentInstance != null)
            currentInstance.OnRuntimeStatUpdated -= OnRuntimeStatUpdated;
        currentInstance = instance;
        if(instance.Damage == 0) 
            damageText.gameObject.SetActive(false);
        else
        {
            damageText.gameObject.SetActive(true);
            damageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = instance.Damage.ToString();
        }
        if(instance.CoinsGenerated == 0) 
            coinText.gameObject.SetActive(false);
        else
        {
            coinText.gameObject.SetActive(true);
            coinText.text = $"${instance.CoinsGenerated}";
        }
        var data = instance.Data;
        activeTabs.ForEach(Destroy);
        nameText.text = data.name;
        descriptionText.text = data.description;
        foreach (var statusEffect in data.statusEffects)
        {
            if (!statusEffect.showInUI) continue;
            AddTab(statusEffect);
        }

        foreach (var buff in data.buffs)
        {
            if (!buff.showInUI) continue;
            AddTab(buff);
        }

        void AddTab(EffectEntry effect)
        {
            var tab = Instantiate(tabPrefab, tabLayout.transform);
            var name = tab.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            name.text = effect.data.name;
            name.color = effect.data.color;
            tab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = effect.data.description;
            activeTabs.Add(tab);
        }

        instance.OnRuntimeStatUpdated += OnRuntimeStatUpdated;
        instance.OnPlayCountUpdated += UpdateDescription;
        instance.OnDamageUpdated += (damage) =>
        {
            if (currentInstance != instance) return;
            damageText.gameObject.SetActive(true);
            damageText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{damage}";
        };
        instance.OnCoinsUpdated += coins =>
        {
            if (currentInstance != instance) return;
            coinText.gameObject.SetActive(true);
            coinText.text = $"${coins}";
        };
        UpdateDescription(instance.PlayCount);
        if (instance.RuntimeStat.HasValue)
        {
            OnRuntimeStatUpdated(instance.RuntimeStat.Value);
            return;
        }
        void UpdateDescription(int playCount)
        {
            descriptionText.text = Regex.Replace(
                descriptionText.text,
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
        runtimeStatText.gameObject.SetActive(false);
    }

    private void OnRuntimeStatUpdated(int value)
    {
        runtimeStatText.gameObject.SetActive(true);
        runtimeStatText.text = $"({currentInstance.Data.FormatRuntimeStat(value)})";
    }
}