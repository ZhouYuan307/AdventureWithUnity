using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    private float slideTackleUsageTimer;
    public float slideTackleDir { get; private set; }
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();   
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(xInput* player.moveSpeed, rb.velocity.y);
        CheckForSlideTackleInput();
        if (xInput == 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }

    private void CheckForSlideTackleInput()
    {

        slideTackleUsageTimer -= Time.deltaTime;

        if (player.IsWallDetected())
        {
            return;
        }



        if (Input.GetKeyDown(KeyCode.LeftControl) && slideTackleUsageTimer < 0)
        {
            slideTackleUsageTimer = player.slideTackleCoolDown;
            //GetAxisRaw:return -1,0,1
            stateMachine.ChangeState(player.slideTackleState);
            return;
        }
    }
}
