using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideTackleState : PlayerState
{
    public PlayerSlideTackleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.slideTackleDuration;
    }

    public override void Exit()
    {
        player.SetVelocity(rb.velocity.x, rb.velocity.y);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.K) && player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        player.SetVelocity(player.slideTackleSpeed * player.facingDir, 0);
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
