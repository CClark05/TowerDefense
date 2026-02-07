using UnityEngine;

public interface IUsesCards
{
    public void AddCard(SkillInstance skillInstance);
    public void RemoveCard(SkillInstance skillData);
    public bool CanAddCard(SkillInstance instance);
}
