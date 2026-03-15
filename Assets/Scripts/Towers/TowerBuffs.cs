using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBuffs : MonoBehaviour
{
    [SerializeField] private GameObject effectUIPrefab;
    [SerializeField] private GridLayoutGroup layoutGroup;
    private SkillContext skillContext;
    private List<EffectUI> activeBuffs = new();
    private EnemyManager enemyManager;
    private void Start()
    {
        skillContext = GetComponent<TowerDataHolder>().SkillContext;
        skillContext.OnBuffAdded += OnBuffAdded;
        skillContext.OnBuffRemoved += OnBuffRemoved;
        enemyManager = EnemyManager.Instance;
        enemyManager.OnWaveComplete += OnWaveComplete;
        enemyManager.OnWaveStarted += OnWaveComplete;
    }
    

    private void OnWaveComplete()
    {
        foreach (var buff in activeBuffs)
        {
            Destroy(buff.gameObject);
        }
        activeBuffs.Clear();
    }

    private void OnBuffRemoved(IBuff buff, int stacks)
    {
        if (EnemyManager.Instance.WaveState == EnemyManager.WaveStates.Complete) return;
        var effectUI = activeBuffs.Find(e => e.data == buff as EffectData);
        effectUI.RemoveStacks(stacks);
        if (effectUI.Stacks <= 0)
        {
            activeBuffs.Remove(effectUI);
            Destroy(effectUI.gameObject);
        }
        
    }
    private void OnBuffAdded(IBuff buff, int stacks)
    {
        if (EnemyManager.Instance.WaveState == EnemyManager.WaveStates.Complete) return;
        var effectUI = activeBuffs.Find(e => e.data == buff as EffectData);
        if (effectUI != null)
        {
            effectUI.AddStacks(stacks);
            return;
        }
        var newEffect = Instantiate(effectUIPrefab, layoutGroup.transform).GetComponent<EffectUI>();
        newEffect.Init(buff as EffectData, stacks);
        activeBuffs.Add(newEffect);
    }

    private void OnDisable()
    {
        enemyManager.OnWaveComplete -= OnWaveComplete;
        enemyManager.OnWaveStarted -= OnWaveComplete;
    }
}
