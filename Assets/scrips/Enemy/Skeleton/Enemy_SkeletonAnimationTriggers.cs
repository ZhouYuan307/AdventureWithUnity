using System.Collections;
using System.Collections.Generic;
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

                hitDir = Sign(hit.GetComponent<Player>().rb.position.x - enemy.rb.position.x);

                hit.GetComponent<Player>().Damage(hitDir);
            }
        }
    }
}
