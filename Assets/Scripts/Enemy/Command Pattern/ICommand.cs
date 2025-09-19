using System.Collections;

public interface ICommand
{
    IEnumerator Execute(IAgent agent);
}

public abstract class EnemyCommand : ICommand
{
    public abstract IEnumerator Execute(IAgent agent);
    protected float duration;

    protected EnemyCommand(float duration)
    {
        this.duration = duration;
    }
}