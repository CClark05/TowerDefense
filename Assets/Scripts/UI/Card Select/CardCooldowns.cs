using System.Collections.Generic;
using System.Linq;

public class CardCooldowns
{
    private Dictionary<SkillData, int> cardCooldowns = new();
    private int cooldownDuration;
    public CardCooldowns(int cooldownDuration)
    {
        this.cooldownDuration = cooldownDuration;
    }
    public void DecreaseCooldowns()
    {
        var keys = cardCooldowns.Keys.ToList();
        foreach (var key in keys)
        {
            cardCooldowns[key]--;
            if (cardCooldowns[key] <= 0)
                cardCooldowns.Remove(key);
        }
    }
    public void SetCooldowns(List<SkillData> cards)
    {
        foreach (var c in cards)
            cardCooldowns[c] = cooldownDuration;
    }
    public bool ContainsKey(SkillData data)
    {
        return cardCooldowns.ContainsKey(data);
    }
}