using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
  
    private float lastAtttackTime;
    private float comboWindow = 2;


    public PlayerPrimaryAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (player.comboCounter > 3 || Time.time >= lastAtttackTime + comboWindow)
            player.comboCounter = 0;

        player.anim.SetInteger("ComboCounter", player.comboCounter);

        stateTimer = 0.1f;

        float attackDirection = player.facingDireciton;

        if (xInput != 0)
            attackDirection = xInput;

        player.SetVelocity(player.attackMove * attackDirection, rb.velocity.y);

    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine(player.CheckBusy(0.25f));

        player.comboCounter++;
        lastAtttackTime = Time.time;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (stateTimer < 0)
            rb.velocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
