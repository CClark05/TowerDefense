using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Exposing Haste Data", menuName = "SkillData/Weakness/Exposing Haste")]
public class ExposingHasteSkillData : SkillData
{
    public int hasteStacksPerEnemy = 1;
    private void OnValidate()
    {
        string weak = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        string haste =  ColorUtility.ToHtmlStringRGB(buffs[0].data.color);
        description = $"While every enemy is <color=#{weak}>Weak</color> gain {hasteStacksPerEnemy} <color=#{haste}>Haste</color> per enemy.";
    }

    public override SkillInstance CreateInstance()
    {
        return new ExposingHasteSkillInstance(this);
    }
}

public class ExposingHasteSkillInstance : SkillInstance<ExposingHasteSkillData>
{
    private EnemyManager enemyManager;
    private int currentStacks;
    public ExposingHasteSkillInstance(ExposingHasteSkillData data) : base(data)
    {
        enemyManager = EnemyManager.Instance;
        enemyManager.OnEnemiesUpdated += OnEnemiesUpdated;
    }

    private void OnEnemiesUpdated()
    {
        if (enemyManager.CurrentEnemies.Count == 0)
        {
            RemoveStacks();
            return;
        }
        foreach (var enemy in enemyManager.CurrentEnemies)
        {
            if (!enemy.GetComponent<IUsesStatusEffects>().PersistentEffectsApplied.ContainsKey(Data.statusEffects[0].data as PersistentStatusEffect))
            {
                RemoveStacks();
                return;
            }
        }

        currentStacks = PlayTwice ? enemyManager.CurrentEnemies.Count * 2 : enemyManager.CurrentEnemies.Count;
        skillContext.AddBuff(Data.buffs[0].data as IBuff, currentStacks * Data.hasteStacksPerEnemy);
        PlayCard();
        void RemoveStacks()
        {
            if(currentStacks >= 1) skillContext.TryRemoveBuff(Data.buffs[0].data as IBuff, currentStacks);
            currentStacks = 0;
            PlayCard();
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        enemyManager.OnEnemiesUpdated -= OnEnemiesUpdated;
    }
}