using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRunner : MonoBehaviour 
{
    public IAgent Agent { get; private set; }
    private Dictionary<BaseState, Coroutine> routines = new();
    public void Init(IAgent agent) => Agent = agent;

    public void Play(BaseState owner, IEnumerable<ICommand> commands)
    {
        Stop(owner);
        routines[owner] = StartCoroutine(Run(commands, owner));
    }

    public bool IsPlaying(BaseState state) => routines.ContainsKey(state);

    private void Stop(BaseState state)
    {
        if (routines.TryGetValue(state, out var routine))
        {
            StopCoroutine(routine);
            routines.Remove(state);
        }
    }
    IEnumerator Run(IEnumerable<ICommand> commands, BaseState owner)
    {
        foreach (var command in commands)
            yield return command.Execute(Agent);
        routines.Remove(owner);
    }
}