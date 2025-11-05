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
                hitDir = Sign(hit.GetComponent<Enemy>().rb.position.x - player.rb.position.x);

                hit.GetComponent<Enemy>().Damage(hitDir);
                hit.GetComponent<CharacterStats>().TakeDamage(player.stats.damage.GetValue());
            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
    }
}
