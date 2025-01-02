using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class Player : Entity
{
    [HideInInspector]
    public PlayerStateMachine stateMachine { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 12.0f;
    public float jumpForce = 12.0f;
    public bool landTrigger;
    public bool isHanging;
    public Vector2 climbOffset;

    [Header("Attack Info")]
    public bool isBusy;
    public float attackMove = 1.0f;
    public int comboCounter;

    [Header("Collision Info")]    
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float ledgeCheckDistance;
    

    [Header("DashInfo")]
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashUseDur;
    public float dashSpeed;
    public float dashDuration;
    public float dashDirection { get; private set; }


    //Move States
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }   
    public PlayerLandState landState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerLedgeGrabState ledgeGrabState { get; private set; }
    public PlayerLedgeClimbState ledgeClimbState { get; private set; }

    // Battle States
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        landState = new PlayerLandState(this, stateMachine, "Land");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        ledgeGrabState = new PlayerLedgeGrabState(this, stateMachine, "LedgeGrab");
        ledgeClimbState = new PlayerLedgeClimbState(this, stateMachine, "LedgeClimb");

        

        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
 
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        landTrigger = false;
        
    }


    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckDashInput();
    }

    protected override void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    // Busy Check
    public IEnumerator CheckBusy(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    // Velocity Check
    public void SetVelocity(float _xVelocity, float _yvelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yvelocity);
        FlipController(_xVelocity);
    }

    // Dash Check
    public void CheckDashInput()
    {

        if (IsWallDetected())
            return;

        dashTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTimer < 0)
        {
            dashTimer = dashUseDur;
            dashDirection = Input.GetAxisRaw("Horizontal");

            if (dashDirection == 0)
                dashDirection = facingDireciton;

            stateMachine.ChangeState(dashState);

        }

    }

    // Collision Check
    public virtual bool IsLedgeDetected() => Physics2D.Raycast(ledgeCheck.position, Vector2.right * facingDireciton, ledgeCheckDistance, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + ledgeCheckDistance, ledgeCheck.position.y));
    }

    // Grab check
    public IEnumerator HangCheck()
    {
        yield return new WaitForSeconds(0.5f);
        isHanging = false;
    }


    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

   
}
