using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static System.Math;

public class Entity : MonoBehaviour
{


    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }

    public CharacterStats stats { get; private set; }

    public CapsuleCollider2D cd {get; private set; }

    public HealthBarUI healthBar { get; private set; }
    
    #endregion
    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDirection;
    [SerializeField] protected float knockbackDuration;
    protected bool isKnocked;


    [Header("Collision info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public Transform attackCheck;
    public float attackCheckRadius;


    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        fx = GetComponent<EntityFX>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
        healthBar = GetComponentInChildren<HealthBarUI>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void BackToDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageFX(Vector2 positionOfHitter)
    {
        stats = GetComponent<CharacterStats>();
        if (stats.GetMissState())
        {
            stats.SetMissState(false);
            return;
        }
        float hitDir = Sign(rb.position.x - positionOfHitter.x);
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback", hitDir);
    }

    public virtual void DamageFX()
    {
        stats = GetComponent<CharacterStats>();
        if (stats.GetMissState())
        {
            stats.SetMissState(false);
            return;
        }
        float hitDir = -facingDir;
        fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback", hitDir);
    }

    protected virtual IEnumerator HitKnockback(float hitDir)
    {
        isKnocked = true;
        rb.velocity = new Vector2(knockbackDirection.x * hitDir, knockbackDirection.y);
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;

    }

    #region Collision
    public virtual bool IsGroundedDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x - wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        onFlipped?.Invoke();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
        {
            Flip();
        }
        else if (_x < 0 && facingRight)
        {
            Flip();
        }
    }
    #endregion

    #region Velocity
    public void SetZeroVelocity() {
        if (isKnocked)
        {
            return;
        }
        rb.velocity = new Vector2(0, 0); 
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
        {
            return;
        }
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion




    public virtual void Die()
    {

    }
}
