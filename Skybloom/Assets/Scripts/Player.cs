using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public PlayerStateMachine stateMachine { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 12.0f;
    public float jumpForce = 12.0f;
    public bool landTrigger;

    [Header("DashInfo")]
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashUseDur;
    public float dashSpeed;
    public float dashDuration;
    public float dashDirection { get; private set; }

    [Header("Collision Info")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Flip Info")]
    public int facingDireciton = 1;
    private bool facingRight = true;

  


    //Components
    public Animator playerAnimator;
    public Rigidbody2D rb;

    //States
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerLandState landState { get; private set; }
    public PlayerDashState dashState { get; private set; }



    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        landState = new PlayerLandState(this, stateMachine, "Land");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
 
    }

    void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);
        landTrigger = false;
        
    }

    
    void Update()
    {
        stateMachine.currentState.Update();
        CheckDash();

        Debug.Log(IsWallDetected());
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();

    }

    public void SetVelocity(float _xVelocity, float _yvelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yvelocity);
        FlipController(_xVelocity);
    }

    public void CheckDash()
    {
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


    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDireciton, wallCheckDistance, whatIsGround);

    public void Flip()
    {
        facingDireciton = facingDireciton * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public void FlipController(float _x)
    {
        if(_x > 0 && !facingRight)
            Flip();
        else if(_x < 0 && facingRight)
            Flip();
    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }

}
