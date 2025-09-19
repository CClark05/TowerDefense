using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreviewUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, descriptionText;
    [SerializeField] private VerticalLayoutGroup statusEffectGroup;
    [SerializeField] private GameObject statusEffectPrefab;
    private List<GameObject> activeStatusEffects = new();
    public void SetSkill(SkillData data)
    {
        activeStatusEffects.ForEach(Destroy);
        int sellPrice = Mathf.FloorToInt(data.price * 0.5f);
        nameText.text = data.name;
        descriptionText.text = data.description;
        foreach (var statusEffect in data.statusEffects)
        {
            if(!statusEffect.showInUI) continue;
            var newEffect = Instantiate(statusEffectPrefab, statusEffectGroup.transform);
            newEffect.GetComponent<TextMeshProUGUI>().text = statusEffect.data.name;
            newEffect.GetComponent<TextMeshProUGUI>().color = statusEffect.data.color;
            newEffect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = statusEffect.data.description;
            activeStatusEffects.Add(newEffect);
        }
        foreach (var buff in data.buffs)
        {
            if(!buff.showInUI) continue;
            var newEffect = Instantiate(statusEffectPrefab, statusEffectGroup.transform);
            newEffect.GetComponent<TextMeshProUGUI>().text = buff.data.name;
            newEffect.GetComponent<TextMeshProUGUI>().color = buff.data.color;
            newEffect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buff.data.description;
            activeStatusEffects.Add(newEffect);
        }
    }
}