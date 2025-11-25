using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText, runtimeStatText;
    [SerializeField] private VerticalLayoutGroup tabLayout;
    [SerializeField] private GameObject tabPrefab;
    private List<GameObject> activeTabs = new();
    private SkillInstance currentInstance;

    public void SetSkill(SkillInstance instance)
    {
        if (currentInstance != null)
            currentInstance.OnRuntimeStatUpdated -= OnRuntimeStatUpdated;
        currentInstance = instance;
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

        if (instance.RuntimeStat.HasValue)
        {
            OnRuntimeStatUpdated(instance.RuntimeStat.Value);
            return;
        }
        runtimeStatText.gameObject.SetActive(false);
    }

    private void OnRuntimeStatUpdated(int value)
    {
        runtimeStatText.gameObject.SetActive(true);
        runtimeStatText.text = $"({currentInstance.Data.FormatRuntimeStat(value)})";
    }
}