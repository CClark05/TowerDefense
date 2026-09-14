using System.Collections.Generic;
using UnityEngine;

public class StrawberryBossStateMachine : BossStateMachine
{
    /*
     * Summon mini strawberries
     * Go invis
     * Teleport
     * Shoot roots at tower
     */
    [SerializeField] private EnemyData miniStrawberryData;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private BuffData stunDebuff;
    private void Start()
    {
        var defaultState = new StrawberryBossDefaultState(facade, 2f);
        var summonState = new StrawberryBossSummonState(facade, 3, miniStrawberryData, 5f);
        var attackState = new StrawberryAttackState(facade, projectilePrefab, stunDebuff);
        stateMachine.SetState(defaultState);
        stateMachine.AddTransition(defaultState, summonState, new FuncPredicate(() => defaultState.IsDone && summonState.IsReady));
        stateMachine.AddTransition(summonState, attackState, new FuncPredicate(() => summonState.IsDone));
        stateMachine.AddTransition(attackState, defaultState, new FuncPredicate(() => attackState.IsDone));
    }
}

public class StrawberryAttackState : BaseState
{
    private GameObject projectilePrefab;
    private List<TowerDataHolder> nearbyTowers = new();
    private BuffData stunDebuff;
    public StrawberryAttackState(IAgent agent, GameObject projectilePrefab, BuffData stunDebuff, float cooldown = 0) : base(agent, cooldown)
    {
        this.projectilePrefab = projectilePrefab;
        this.stunDebuff = stunDebuff;
    }

    public override void OnEnter()
    {
        Debug.Log("Strawberry attack state entered");
        runner.Play(this, new ICommand[]
        {
            new StopMovementCommand(),
            new GetNearbyTowersCommand(nearbyTowers, 1),
        });
    }

    public override void OnExit()
    {
        runner.Play(this, new ICommand[]
        {
            new ShootProjectileCommand(projectilePrefab, nearbyTowers[0].transform, 5f, tower =>
            {
                tower.SkillContext.AddBuff(stunDebuff as IBuff, 3);
            }),
            new ResetSpeedCommand(),
        });
    }
}
public class StrawberryBossSummonState : BaseState {
    
    private int summons;
    private EnemyData miniStrawberryData;
    private List<TowerDataHolder> nearbyTowers = new();
    public StrawberryBossSummonState(IAgent agent, int summons, EnemyData miniStrawberryData, float cooldown) : base(agent, cooldown)
    {
        this.summons = summons;
        this.miniStrawberryData = miniStrawberryData;
    }

    public override void OnEnter()
    {
        Debug.Log("Strawberry boss summon state entered");
        runner.Play(this, new ICommand[]
        {
            new StopMovementCommand(),
            new SpawnEnemiesCommand(miniStrawberryData, summons, agent.Transform.position, 0.5f),
            new ResetSpeedCommand(),
            new GetTowersUntilFoundCommand(5f, nearbyTowers),
        });
    }
}
public class StrawberryBossDefaultState : BaseState
{
    private float duration;
    public StrawberryBossDefaultState(IAgent agent, float duration, float cooldown = 0) : base(agent, cooldown)
    {
        this.duration = duration;
    }
    public override void OnEnter()
    {
        Debug.Log("Strawberry boss default state entered");
        runner.Play(this, new ICommand[]
        {
            new WaitCommand(duration),
        });
    }
}