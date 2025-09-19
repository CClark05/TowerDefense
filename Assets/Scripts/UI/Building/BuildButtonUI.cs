using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuildButtonUI : MonoBehaviour
{
    [SerializeField] private BuildableObjectData data;
    [SerializeField] private GameObject materialsRequiredUI;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject materialAmountPrefab;
    private Button_Hover button;
    public BuildableObjectData Data => data;
    
    private void Awake()
    {
        button = GetComponent<Button_Hover>();
    }

    private void Start()
    {
        button.OnHover += () =>
        {
            materialsRequiredUI.SetActive(true);
            nameText.gameObject.SetActive(false);
        };
        button.OnLeaveHover += () =>
        {
            materialsRequiredUI.SetActive(false);
            nameText.gameObject.SetActive(true);
        };
        foreach (var (material, amount) in data.CostDictionary)
        {
            GameObject newMaterial = Instantiate(materialAmountPrefab, materialsRequiredUI.transform);
            newMaterial.GetComponent<Image>().sprite = material.prefab.GetComponent<SpriteRenderer>().sprite;
            newMaterial.GetComponent<Image>().color = material.prefab.GetComponent<SpriteRenderer>().color;
            newMaterial.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = amount.ToString();
        }
    }
}

