using Unity.VisualScripting;
using UnityEngine;

public class SlimeSummonState : BaseState
{
    private float duration;
    private int summonsPerCluster;
    private EnemyData summonData;
    public SlimeSummonState(IAgent agent, float duration, int summonsPerCluster, EnemyData summonData,float cooldown) : base(agent, cooldown)
    {
        this.duration = duration;
        this.summonsPerCluster = summonsPerCluster;
        this.summonData = summonData;
    }

    public override void OnEnter()
    {
        Debug.Log("Enter summon");
        runner.Play(this, new ICommand[]
        {
            new StopMovementCommand(0.25f),
            new WaitCommand(0.5f),
            new SpawnEnemyCommand(summonData, summonsPerCluster, agent.Transform.position),
            new WaitCommand(1),
            new SpawnEnemyCommand(summonData, summonsPerCluster, agent.Transform.position),
            new ResetSpeedCommand(0.25f),
        });
    }
    
}
