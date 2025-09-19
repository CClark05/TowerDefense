using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stacksText;
    public int Stacks { get; private set; }
     public EffectData data { get; private set; }

    public void Init(EffectData data, int stacks)
    {
        this.data = data;
        GetComponent<Image>().sprite = data.sprite;
        this.Stacks = stacks;
        stacksText.text = stacks.ToString();
    }

    public void AddStacks(int amount)
    {
        Stacks += amount;
        stacksText.text = Stacks.ToString();
    }

    public void RemoveStacks(int amount)
    {
        Stacks -= amount;
        stacksText.text = Stacks.ToString();
    }
}
