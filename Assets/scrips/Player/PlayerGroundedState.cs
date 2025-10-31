using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerGroundedState : PlayerState
{

    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            stateMachine.ChangeState(player.blackHoleState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword())
        {
            stateMachine.ChangeState(player.aimSwordState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C)) 
        {
            stateMachine.ChangeState(player.counterAttackState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            stateMachine.ChangeState(player.primaryAttackState);
            return;
        }

        if (!player.IsGroundedDetected()) 
        {
            stateMachine.ChangeState(player.airState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundedDetected()) { 
            stateMachine.ChangeState(player.jumpState);
            return;
        }

    }
    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }
}
