using System;
using UnityEngine;

public class SlimeBossStateMachine : BossStateMachine
{
    [SerializeField] private BuffData stunDebuff;
    [SerializeField] private AnimationClip jumpAnimation, shockwaveAnimation;
    [SerializeField] private EnemyData slimeMinionData;
    private void Start()
    {
        var healthPredicate = new HealthPercentagePredicate(facade.GetComponent<IUsesHealth>());
        var defaultState = new SlimeDefaultState(facade, 3);
        var jumpState = new SlimeJumpState(facade, 1, 2, stunDebuff, 1, 4, jumpAnimation, shockwaveAnimation, 5);
        var summonState = new SlimeSummonState(facade, 2, 4, slimeMinionData, 3, 3);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, summonState, healthPredicate);
        stateMachine.AddTransition(summonState, defaultState, new FuncPredicate(() => summonState.IsDone));
        stateMachine.AddTransition(defaultState, jumpState, new FuncPredicate(() => defaultState.IsDone && jumpState.IsReady));
        stateMachine.AddTransition(jumpState, defaultState, new FuncPredicate(() => jumpState.IsDone));
        //default
        //if health predicate -> summon -> default
        //if near tower -> jump -> default
    }   
}