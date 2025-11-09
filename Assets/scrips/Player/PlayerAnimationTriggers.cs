using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private float hitDir;
    private Player player => GetComponentInParent<Player>();

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats  _target = hit.GetComponent<EnemyStats>();
                player.stats.DoDamage(_target);
                if (_target.GetMissState())
                {
                    _target.SetMissState(false);
                    //do miss animation
                    return;
                }
                hit.GetComponent<Enemy>().DamageFX(player.rb.position);
                
            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
