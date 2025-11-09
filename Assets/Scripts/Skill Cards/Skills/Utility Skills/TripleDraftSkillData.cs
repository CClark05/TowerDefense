using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Draft Data", menuName = "SkillData/Utility/Draft")]
public class DraftSkillData : SkillData
{
    public int cardsDrawn = 3;
    public SkillRegistry skillRegistry;
    public CardRaritySettings cardRaritySettings;
    private void OnValidate()
    {
        description = $"Draw 3 random cards into your hand if you have room. Self destructs after use.";
    }

    public override SkillInstance CreateInstance()
    {
        return new TripleDraftSkillInstance(this);
    }
}
public class TripleDraftSkillInstance : SkillInstance<DraftSkillData>, ISelfDestructs, ITowerCardReceivedModifier
{
    public TripleDraftSkillInstance(DraftSkillData data) : base(data)
    {
    }

    public Action OnSelfDestruct { get; set; }
    public void Apply(TowerWaveData towerWaveData)
    {
        PlayCard();
        List<SkillData> drawnCards = CardRarityPicker.PickCards(Data.skillRegistry.Skills, Data.cardRaritySettings, Data.cardsDrawn);
        var instances = drawnCards.Select(c => c.CreateInstance()).ToList();
        InventoryUI.Instance.AddCards(instances);
        towerWaveData.removedCards.Add(this);
        OnSelfDestruct?.Invoke();
    }
    public void Remove(TowerWaveData towerWaveData)
    {
        
    }
}