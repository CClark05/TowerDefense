using System;
using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;

    private void Start()
    {
        coinsText.text = PlayerInventory.Instance.Coins.ToString();
        PlayerInventory.Instance.OnCoinsUpdated += coins =>
        {
            coinsText.text = coins.ToString();
        };
    }
}
