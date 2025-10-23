using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{


    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;


    public bool isBusy {  get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; }

    [Header("Slide tackle info")]
    [SerializeField]private float slideTackleCoolDown;
    private float slideTackleUsageTimer;
    public float slideTackleSpeed;
    public float slideTackleDuration;
    public float slideTackleDir { get; private set; }






    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }

    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }

    public PlayerAirState airState { get; private set; }

    public PlayerWallSlideState wallSlideState { get; private set; }

    public PlayerWallJumpState wallJumpState { get; private set; }

    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttackState { get; private set; }

    public PlayerCounterAttackState counterAttackState { get; private set; }

    public PlayerSlideTackleState slideTackleState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "WallJump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        slideTackleState = new PlayerSlideTackleState(this, stateMachine, "SlideTackle");

    }

    protected override void Start()
    {

        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
        CheckForSlideTackleInput();
    }

    //使用StartCoroutine(name, agv)来调用
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        //Debug.Log("isBusy");

        yield return new WaitForSeconds(_seconds);//当检查条件为真时才继续执行

        //Debug.Log("notBusy");
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {



        if (IsWallDetected())
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.V) && SkillManager.instance.dash.TryUseSkill())
        {

            //GetAxisRaw:return -1,0,1
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
            {
                dashDir = facingDir;
            }
            stateMachine.ChangeState(dashState);
        }
    }

    private void CheckForSlideTackleInput()
    {

        slideTackleUsageTimer -= Time.deltaTime;

        if (IsWallDetected())
        {
            return;
        }



        if (Input.GetKeyDown(KeyCode.LeftShift) && slideTackleUsageTimer < 0)
        {
            slideTackleUsageTimer = slideTackleCoolDown;
            //GetAxisRaw:return -1,0,1
            slideTackleDir = Input.GetAxisRaw("Horizontal");
            if (slideTackleDir == 0)
            {
                slideTackleDir = facingDir;
            }
            stateMachine.ChangeState(slideTackleState);
        }
    }

}
