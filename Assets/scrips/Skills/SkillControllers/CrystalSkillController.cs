using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class CrystalSkillController : SkillController
{
    private float hitDir;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTimer;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestTarget;
    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = FindClosestEnemy(transform);
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            crystalCompleted();
        }

        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                crystalCompleted();
                canMove = false;
            }
        }

        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hitDir = Sign(hit.GetComponent<Enemy>().rb.position.x - transform.position.x);

                hit.GetComponent<Enemy>().Damage(hitDir);
            }
        }
    }

    public void crystalCompleted()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}
