using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //BugLog
    //BUG1跳跃状态无法触发滑墙（即上升时）
    //[FIXED]BUG2触发滑墙瞬间按ad转向会导致滑墙动画翻转(原因是airstate在执行切换状态机操作后会继续执行update剩余内容，需要提前返回)
    //[FIXED]BUG3半空下墙会导致持续滑行(在idlestate进入时速度置零即可)
    //[FIXED]BUG4状态紊乱，由于return的退出作用域问题，子类MoveState的update退出不及时，导致攻击状态获得速度
    //[FIXED]BUG5连招期间无法转向，必须停下攻击

    [Header("Attack details")]
    public Vector2[] attackMovement;



    public bool isBusy {  get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("Dash info")]
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; }

    






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
        dashUsageTimer -= Time.deltaTime;


        if (IsWallDetected())
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
        {
            dashUsageTimer = dashCooldown;
            //GetAxisRaw:return -1,0,1
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0)
            {
                dashDir = facingDir;
            }
            stateMachine.ChangeState(dashState);
        }
    }

}
