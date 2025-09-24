using UnityEngine;

public abstract class BaseState : IState
{
    protected CommandRunner runner;
    public bool IsDone => !runner.IsPlaying(this);
    protected IAgent agent;
    private float cooldown;
    private float readyAt;
    public bool IsReady => Time.time >= readyAt;
    protected BaseState(IAgent agent, float cooldown = 0f)
    {
        this.agent = agent;
        this.cooldown = cooldown;
        runner = (agent as Component).GetComponent<CommandRunner>() ??
                 (agent as Component).gameObject.AddComponent<CommandRunner>();
        runner.Init(agent);
    }
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
        readyAt = Time.time + cooldown;
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Update()
    {
    }
}