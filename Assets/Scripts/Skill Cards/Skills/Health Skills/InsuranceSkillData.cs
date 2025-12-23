using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Insurance Data", menuName = "SkillData/Health/Insurance")]
public class InsuranceSkillData : SkillData
{
    public int restoredLives = 5;
    private void OnValidate()
    {
        description = $"When you reach 0 lives, gain +{restoredLives} lives. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new InsuranceSkillInstance(this);
    }
}
public class InsuranceSkillInstance : SkillInstance<InsuranceSkillData>, IOnLivesUpdated, ISelfDestructs, IPlayCountPolicy<IOnLivesUpdated>
{
    public InsuranceSkillInstance(InsuranceSkillData data) : base(data)
    {
    }

    public void OnLivesUpdated(int lives)
    {
        if(lives <= 0)
        {
            PlayCard();
            PlayerLife.Instance.AddLives(Data.restoredLives * PlayCount);
            OnSelfDestruct?.Invoke();
        }
    }

    public Action OnSelfDestruct { get; set; }
    public int SetPlayCount() => 1;
}