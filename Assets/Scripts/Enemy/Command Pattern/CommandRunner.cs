using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRunner : MonoBehaviour 
{
    public IAgent Agent { get; private set; }
    private Coroutine routine;
    private BaseState stateOwner;
    public void Init(IAgent agent) => Agent = agent;

    public void Play(BaseState owner, IEnumerable<ICommand> commands)
    {
        Stop();
        stateOwner = owner;
        routine = StartCoroutine(Run(commands, owner));
    }

    public bool IsPlaying(BaseState state)
    {
        return routine != null && state == stateOwner;
    }
    private void Stop()
    {
        if (routine == null) return;
        StopCoroutine(routine);
        routine = null;
        stateOwner = null;
    }

    IEnumerator Run(IEnumerable<ICommand> commands, BaseState owner)
    {
        stateOwner = owner;
        foreach (var command in commands)
            yield return command.Execute(Agent);
        routine = null;
    }
}