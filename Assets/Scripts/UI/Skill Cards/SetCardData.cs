using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetCardData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText;
    //[SerializeField] private VerticalLayoutGroup statusEffectGroup;
    //[SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private GameObject sideTabPrefab;
    [SerializeField] private SkillData skillData;
    [SerializeField] private Image skillIconImage;
    [SerializeField] private VerticalLayoutGroup tabGroup;
    public SkillData SkillData => skillData;
    public SkillInstance SkillInstance { get; private set; }
    //private List<GameObject> currentStatusEffects = new();
    private List<GameObject> currentTabs = new();
    public InterfaceReference<ICardUI> cardUI;
    private void Start()
    {
        //UpdateVisual();
        if(skillData.icon != null)
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
        foreach (var effect in skillData.statusEffects)
        {
            if(!effect.showInUI) continue;
            AddTab(effect);
        }
        foreach (var buff in skillData.buffs)
        {
            if(!buff.showInUI) continue;
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
    }

    public void DisableTabs()
    {
        foreach (var tab in currentTabs)
            tab.SetActive(false);
    }
}