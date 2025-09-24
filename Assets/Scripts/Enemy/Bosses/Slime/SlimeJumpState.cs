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
    private AnimationClip jumpAnimation;
    private AnimationClip shockwaveAnimation;
    public SlimeJumpState(IAgent agent, float duration, float jumpHeight, BuffData stunDebuff, int stunStacks, float stunRadius, AnimationClip jumpAnimation, AnimationClip shockwaveAnimation, float cooldown) : base(agent, cooldown)
    {
        this.jumpHeight = jumpHeight;
        this.stunDebuff = stunDebuff;
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
        base.OnExit();
        Transform shockWave = agent.Transform.GetComponent<AnimationVisualGroup>().TryGet(0);
        float scaleTo = HelperMethods.ScaleForRadius(shockWave.GetComponent<SpriteRenderer>(), stunRadius);
        agent.Require<IAnimationPlayer>().Play(shockwaveAnimation, shockWave, AnimPlayMode.Auto, 0.5f, 
            args: new AnimArgs{scaleTo = new Vector2(scaleTo,scaleTo)});
        CameraShake.Shake(Camera.main.transform, 0.2f, 0.4f);
        runner.Play(this, new ICommand[]
        {
            new StunCommand(stunDebuff, stunStacks, nearbyTowers)
        });
    }
    
}