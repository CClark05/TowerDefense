using UnityEngine;

[CreateAssetMenu(fileName = "Stunned Debuff", menuName = "Debuffs/Stunned")]
public class StunnedDebuff : BuffData, IBuff
{
    public float duration = 0.5f;
    private void OnValidate()
    {
        description = $"Tower is unable to shoot for {duration} second(s) per stack.";
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.stunnedDuration += duration;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.stunnedDuration -= duration;
    }
}