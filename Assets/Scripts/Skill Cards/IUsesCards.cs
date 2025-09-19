using UnityEngine;

public interface IUsesCards
{
    public bool TryAddCard(SkillData skillData);
    public void RemoveCard(SkillData skillData);
}
