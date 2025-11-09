using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static System.Math;

public class Enemy_SkeletonAnimationTriggers : MonoBehaviour
{

    private float hitDir;

    private Enemy_Skeleton enemy => GetComponentInParent<Enemy_Skeleton>();
    
    private void AinimationTrigger() { enemy.AnimationFinishTrigger(); }



    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats _target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(_target);
                if (_target.GetMissState())
                {
                    _target.SetMissState(false);
                    //do miss animation
                    return;
                }
                hit.GetComponent<Player>().DamageFX(enemy.rb.position);
            }
        }
    }

    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();

    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
