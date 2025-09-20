using System.Collections.Generic;
using UnityEngine;

public class SlimeJumpState : BaseState
{
    private float jumpHeight;
    private BuffData stunDebuff;
    private int stunStacks;
    private float duration;
    private float stunRadius;
    private List<TowerDataHolder> nearbyTowers = new();
    private float readyAt;
    public bool IsReady => Time.time >= readyAt;
    private float jumpCooldown;
    public SlimeJumpState(IAgent agent, float duration, float jumpHeight, float jumpCooldown, BuffData stunDebuff, int stunStacks, float stunRadius) : base(agent)
    {
        this.jumpHeight = jumpHeight;
        this.stunDebuff = stunDebuff;
        this.jumpCooldown = jumpCooldown;
        this.stunStacks = stunStacks;
        this.duration = duration;
        this.stunRadius = stunRadius;
    }
    public override void OnEnter()
    {
        Debug.Log("enter jump");
        runner.Play(this,new ICommand[]
        {
            new JumpCommand(duration, jumpHeight),
            new GetTowersCommand(stunRadius, nearbyTowers)
        });
    }

    public override void OnExit()
    {
        runner.Play(this, new ICommand[]
        {
            new StunCommand(stunDebuff, stunStacks, nearbyTowers)
        });
        readyAt = Time.time + jumpCooldown;
    }
    
}