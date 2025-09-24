using UnityEngine;

public class SlimeSummonState : BaseState
{
    private float duration;
    private int summons;
    private EnemyData summonData;
    public SlimeSummonState(IAgent agent, float duration, int summons, EnemyData summonData,float cooldown) : base(agent, cooldown)
    {
        this.duration = duration;
        this.summons = summons;
        this.summonData = summonData;
    }

    public override void OnEnter()
    {
        Debug.Log("Enter summon");
        runner.Play(this, new ICommand[]
        {
            new StopMovementCommand(0.25f),
            new WaitCommand(0.5f),
            new SpawnEnemyCommand(summonData, summons, agent.Transform.position),
            new WaitCommand(1),
            new ResetSpeedCommand(0.25f),
        });
    }
    
}
