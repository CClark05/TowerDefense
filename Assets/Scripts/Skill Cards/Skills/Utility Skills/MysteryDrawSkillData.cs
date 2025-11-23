using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Draw Data", menuName = "SkillData/Utility/Mystery Draw")]
public class MysteryDrawSkillData : SkillData
{
    public SkillRegistry skillRegistry;
    public CardRaritySettings cardRaritySettings;

    private void OnValidate()
    {
        description = "On wave end draw a random card into your hand if you have room.";
    }

    public override SkillInstance CreateInstance()
    {
        return new MysteryDrawSkillInstance(this);
    }
}

public class MysteryDrawSkillInstance : SkillInstance<MysteryDrawSkillData>, ITowerWaveEndModifier
{
    public MysteryDrawSkillInstance(MysteryDrawSkillData data) : base(data)
    {
    }

    public void Modify(TowerWaveData towerWaveData)
    {
        PlayCard();
        var card = CardRarityPicker.PickCards(Data.skillRegistry.Skills, Data.cardRaritySettings, 1);
        InventoryUI.Instance.AddCard(card[0].CreateInstance());
    }
}
