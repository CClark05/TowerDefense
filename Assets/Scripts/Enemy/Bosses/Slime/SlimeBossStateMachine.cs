using System;
using UnityEngine;

public class SlimeBossStateMachine : BossStateMachine
{
    private EnemyFacade facade;
    [SerializeField] private BuffData stunDebuff;
    [SerializeField] private AnimationClip jumpAnimation, shockwaveAnimation;
    [SerializeField] private EnemyData slimeMinionData;
    private new void Awake()
    {
        base.Awake();
        facade = GetComponent<EnemyFacade>();
    }
    private void Start()
    {
        var defaultState = new SlimeDefaultState(facade, 3);
        var jumpState = new SlimeJumpState(facade, 1, 2, stunDebuff, 1, 3, jumpAnimation, shockwaveAnimation, 3);
        var summonState = new SlimeSummonState(facade, 2, 3, slimeMinionData, 3);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, summonState, new FuncPredicate(() => defaultState.IsDone && summonState.IsReady));
        stateMachine.AddTransition(summonState, defaultState, new FuncPredicate(() => summonState.IsDone));
    }   
}