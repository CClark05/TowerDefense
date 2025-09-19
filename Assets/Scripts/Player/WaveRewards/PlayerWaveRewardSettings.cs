using UnityEngine;
[CreateAssetMenu(fileName = "Wave Rewards Settings", menuName = "Player/Wave Rewards Settings")]
public class PlayerWaveRewardSettings : ScriptableObject
{
    public float InterestRate = 0.1f;
    public int coinsPerMove = 1;
}
