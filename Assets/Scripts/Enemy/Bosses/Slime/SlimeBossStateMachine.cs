using System;
using UnityEngine;

public class SlimeBossStateMachine : BossStateMachine
{
    private EnemyFacade facade;
    [SerializeField] private BuffData stunDebuff;
    private new void Awake()
    {
        base.Awake();
        facade = GetComponent<EnemyFacade>();
    }
    private void Start()
    {
        var defaultState = new SlimeDefaultState(facade, 3);
        var jumpState = new SlimeJumpState(facade, 1, 2, 3,stunDebuff, 1, 3);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, jumpState, new FuncPredicate(() => defaultState.IsDone && jumpState.IsReady));
        stateMachine.AddTransition(jumpState, defaultState, new FuncPredicate(() => jumpState.IsDone));
    }
}