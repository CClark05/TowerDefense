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
    private AnimationClip jumpAnimation;
    private AnimationClip shockwaveAnimation;
    public SlimeJumpState(IAgent agent, float duration, float jumpHeight, float jumpCooldown, BuffData stunDebuff, int stunStacks, float stunRadius, AnimationClip jumpAnimation, AnimationClip shockwaveAnimation) : base(agent)
    {
        this.jumpHeight = jumpHeight;
        this.stunDebuff = stunDebuff;
        this.jumpCooldown = jumpCooldown;
        this.stunStacks = stunStacks;
        this.duration = duration;
        this.stunRadius = stunRadius;
        this.jumpAnimation = jumpAnimation;
        this.shockwaveAnimation = shockwaveAnimation;
    }
    public override void OnEnter()
    {
        Debug.Log("enter jump");
        agent.Require<IAnimationPlayer>().Play(jumpAnimation, agent.Transform, AnimPlayMode.Once, duration);
        nearbyTowers.Clear();
        runner.Play(this,new ICommand[]
        {
            new JumpCommand(duration, jumpHeight),
            new GetTowersCommand(stunRadius, nearbyTowers)
        });
    }

    public override void OnExit()
    {
        agent.Require<IAnimationPlayer>().Play(shockwaveAnimation, agent.Transform.GetComponent<AnimationVisualGroup>().TryGet(0));
        runner.Play(this, new ICommand[]
        {
            new StunCommand(stunDebuff, stunStacks, nearbyTowers)
        });
        readyAt = Time.time + jumpCooldown;
    }
    
}