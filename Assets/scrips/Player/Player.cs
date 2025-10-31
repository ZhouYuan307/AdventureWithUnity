using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{


    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;
    public bool isBusy { get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; }

    [Header("Slide tackle info")]
    [SerializeField]public float slideTackleCoolDown;
    public float slideTackleSpeed;
    public float slideTackleDuration;




    public SkillManager skill {  get; private set; }

    public GameObject sword { get; private set; }


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

    public PlayerCatchSwordState catchSwordState { get; private set; }

    public PlayerAimSwordState aimSwordState { get; private set; }

    public PlayerBlackHoleState blackHoleState { get; private set; }
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
        slideTackleState = new PlayerSlideTackleState(this, stateMachine, "SlideTackle");

        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHoleState = new PlayerBlackHoleState(this, stateMachine, "Jump");

    }

    protected override void Start()
    {

        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();

    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public void ExitBlackHoleAblity()
    {
        stateMachine.ChangeState(airState);
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


        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.TryUseSkill())
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



}
