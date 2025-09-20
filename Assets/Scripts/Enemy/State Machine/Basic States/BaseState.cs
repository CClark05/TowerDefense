using UnityEngine;

public abstract class BaseState : IState
{
    protected CommandRunner runner;
    public bool IsDone => !runner.IsPlaying(this);
    protected IAgent agent;
    protected BaseState(IAgent agent)
    {
        this.agent = agent;
        runner = (agent as Component).GetComponent<CommandRunner>() ??
                 (agent as Component).gameObject.AddComponent<CommandRunner>();
        runner.Init(agent);
    }
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Update()
    {
    }
}