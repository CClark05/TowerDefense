using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SetCardData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText, sellPriceText;
    [SerializeField] private VerticalLayoutGroup statusEffectGroup;
    [SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private SkillData skillData;
    public SkillData SkillData => skillData;
    public SkillInstance SkillInstance { get; private set; }
    public int SellPrice { get; private set; }
    private List<GameObject> currentStatusEffects = new();
    private void Awake()
    {
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        SellPrice = Mathf.FloorToInt(skillData.price * 0.5f);
        nameText.text = skillData.name;
        descriptionText.text = skillData.description;
        sellPriceText.text = $"$ {SellPrice}";
        foreach (var effect in currentStatusEffects)
        {
            Destroy(effect);
        }
        currentStatusEffects.Clear();
        foreach (var effect in skillData.statusEffects)
        {
            if(!effect.showInUI) continue;
            var newEffect = Instantiate(statusEffectPrefab, statusEffectGroup.transform);
            newEffect.GetComponent<TextMeshProUGUI>().text = effect.data.name;
            newEffect.GetComponent<TextMeshProUGUI>().color = effect.data.color;
            newEffect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = effect.data.description;
            currentStatusEffects.Add(newEffect);
        }
        foreach (var buff in skillData.buffs)
        {
            if(!buff.showInUI) continue;
            var newEffect = Instantiate(statusEffectPrefab, statusEffectGroup.transform);
            newEffect.GetComponent<TextMeshProUGUI>().text = buff.data.name;
            newEffect.GetComponent<TextMeshProUGUI>().color = buff.data.color;
            newEffect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buff.data.description;
            currentStatusEffects.Add(newEffect);
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
}