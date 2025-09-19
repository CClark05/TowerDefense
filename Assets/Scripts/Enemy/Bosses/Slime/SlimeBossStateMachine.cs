using System;
using UnityEngine;

public class SlimeBossStateMachine : BossStateMachine
{
    private EnemyFacade facade;
    private new void Awake()
    {
        base.Awake();
        facade = GetComponent<EnemyFacade>();
    }
    private void Start()
    {
        var sprintState = new SprintState(facade, 3, 3f);
        var idleState = new IdleState(facade, 1, 0.6f);
        var defaultState = new DefaultState(facade, 3);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, sprintState, new FuncPredicate(() => defaultState.IsDone));
        stateMachine.AddTransition(idleState, defaultState, new FuncPredicate(() => idleState.IsDone));
        stateMachine.AddTransition(sprintState, idleState, new FuncPredicate(() => sprintState.IsDone));
    }
}