using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    private int amount;
    private void Start()
    {
        amount = PlayerInventory.Instance.Coins;
        coinsText.text = amount.ToString();
        GetComponent<CoinsUIAnimation>().OnPopupComplete += AnimateText;
    }

    private void AnimateText(int target)
    {
        float duration = 0.3f;
        DOTween.To(() => amount, x =>
        {
            coinsText.text = x.ToString();
        }, amount + target, duration).OnComplete(() => amount += target).SetUpdate(true);
    }
}