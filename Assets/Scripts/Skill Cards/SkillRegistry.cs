using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName="SkillData/Skill Registry")]
public class SkillRegistry : ScriptableObject
{
    public List<SkillData> Skills = new();
    public HashSet<SkillData> CurrentSkills = new();

    public void AddNewSkill(SkillData data)
    {
        CurrentSkills.Add(data);
    }
}