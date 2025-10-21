using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //BUG3 FIX BEGIN
        player.SetZeroVelocity();
        //BUG3 FIX END
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        ////阻止贴墙跑
        //if (xInput == player.facingDir && player.IsWallDetected())
        //{
        //    return;
        //}

        if (xInput != 0 && !player.isBusy)//忙于攻击不能立刻转到move，防止攻击时平移,同时导致了攻击结束后只能原地跳跃的bug
        {
            stateMachine.ChangeState(player.moveState);
        }

    }
}
