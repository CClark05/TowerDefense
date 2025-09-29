using System;
using UnityEngine;
[RequireComponent(typeof(EnemyFacade))]
public abstract class BossStateMachine : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected EnemyFacade facade;
    protected void Awake()
    {
        stateMachine = new StateMachine();
        facade = GetComponent<EnemyFacade>();
    }

    protected void Update()
    {
        stateMachine.Update();
    }

    protected void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
}