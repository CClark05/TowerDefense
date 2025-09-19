using System;
using UnityEngine;

public abstract class BossStateMachine : MonoBehaviour
{
    protected StateMachine stateMachine;

    protected void Awake()
    {
        stateMachine = new StateMachine();
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