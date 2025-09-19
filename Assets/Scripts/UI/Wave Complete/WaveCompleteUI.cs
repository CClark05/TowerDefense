using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WaveCompleteUI : Singleton<WaveCompleteUI>
{
    [SerializeField] private Button_Base continueButton;
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI interestText, leftoverMovesText, interestAmount, leftoverMovesReward, baseRewardAmount, totalAmount, waveText;
    private WaveRewards waveRewards;
    public event Action<int> OnCollectedReward; 

    private void Start()
    {
        continueButton.OnClick.AddListener(() =>
        {
            OnCollectedReward?.Invoke(waveRewards.TotalReward);
            background.SetActive(false);
        });
        PlayerInventory.Instance.OnWaveRewardsCalculated += OnWaveRewards;
        background.SetActive(false);
    }

    private void OnWaveRewards(WaveRewards reward)
    {
        waveRewards = reward;
        background.SetActive(true);
        interestText.text = $"Interest ({reward.InterestRate * 100}% of ${reward.InitialCoins})";
        interestAmount.text = $"${reward.InterestAmount}";
        leftoverMovesText.text = $"Leftover Moves ({reward.LeftoverMoves})";
        leftoverMovesReward.text = $"${reward.LeftoverMovesReward}";
        baseRewardAmount.text = $"${reward.BaseReward}";
        totalAmount.text = $"${reward.TotalReward}";
        waveText.text = $"WAVE {reward.WaveNumber}";
    }
    
}