using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoxJumpState
{
    IDLE = 0,
    JUMP = 1,
    FALL = 2,
}
public class PlayerMovement : MonoBehaviour
{
    [Header("Booleans")]
    [SerializeField] bool canMove = true;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] bool isGrounded;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    public float horizontalMove;
    public float movementSmoothing;
    [SerializeField] float runSpeed = 400;
    [SerializeField] Vector3 velocity = Vector3.zero;

    [Header("Jumping")]
    [SerializeField] bool isJumping;
    [SerializeField] float jumpForce;
    [SerializeField] float maxJumpTime;
    [SerializeField] float jumpTimeCounter;
    [SerializeField] Transform groundCheck;
    [SerializeField] FoxJumpState currentJumpState;

    private float lastYPosition;
    private float fallSpeedMultiplier = 2.5f;
    private float maxFallSpeedMultiplier = 5f;

    [Header("Advanced Jumping")]
    [SerializeField] bool wallJump;
    [SerializeField] bool canDoubleJump;
    [SerializeField] bool onWall;
    [SerializeField] bool isWallSliding;
    [SerializeField] float slideSpeed;
    [SerializeField] float wallJumpXForce;
    [SerializeField] float wallJumpYForce;
    [SerializeField] Transform wallCheck;
    private float wallCheckDistance = .25f;

    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }
    public FoxJumpState CurrentJumpState
    {
        get { return currentJumpState; }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        lastYPosition = transform.position.y;
    }
    private void Update()
    {
        HandleMovementInputs();
        HandleJumpInputs();
    }
    private void HandleMovementInputs()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }
    private void HandleJumpInputs()
    {
        if (!isJumping)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (canDoubleJump)
                {
                    canDoubleJump = isGrounded;
                    isJumping = true;
                    jumpTimeCounter = maxJumpTime;
                    if (onWall) wallJump = true;
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space) && jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;
            }
            else isJumping = false;
            if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpTimeCounter = 0;
                isJumping = false;
            }
        }
    }
    private void FixedUpdate()
    {
        bool wasGrounded = CheckIfGrounded();
        if (isGrounded && !isJumping) HandleLanding(wasGrounded);
        else
        {
            HandleWallColision();
            HandleAirState();
        }
        lastYPosition = transform.position.y;
        anim.SetInteger("jumpState", (int)currentJumpState);
        HandleMovement();
    }
    private void HandleWallColision()
    {
        onWall = Physics2D.Raycast(wallCheck.position, new Vector2(transform.localScale.x, 0), wallCheckDistance, GameLayers.i.GroundLayer);
        Debug.DrawRay(wallCheck.position, new Vector2(transform.localScale.x, 0) * wallCheckDistance, Color.red);
        if (onWall) canDoubleJump = true;
    }
    private bool CheckIfGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, GameLayers.i.GroundCheckDistance, GameLayers.i.GroundLayer);
        return wasGrounded;
    }
    private void HandleAirState()
    {
        if (transform.position.y > lastYPosition)
        {
            if (currentJumpState == FoxJumpState.IDLE)
            {
                currentJumpState = FoxJumpState.JUMP;
            }
        }
        else if (transform.position.y < lastYPosition)
        {
            if (currentJumpState == FoxJumpState.JUMP)
            {
                currentJumpState = FoxJumpState.FALL;
            }
        }
    }
    private void HandleLanding(bool wasGrounded)
    {
        if (!wasGrounded)
        {
            isJumping = false;
            currentJumpState = FoxJumpState.IDLE;
            canDoubleJump = true;
            wallJump = false;
            onWall = false;
            if (isWallSliding)
            {
                GetComponent<SpriteRenderer>().flipX = false;
                isWallSliding = false;
            }
        }
    }
    private void HandleMovement()
    {
        if (canMove)
        {
            if (isGrounded)
            {
                Vector2 targetVelocity = new Vector2(horizontalMove * runSpeed * Time.fixedDeltaTime, rb.velocity.y);
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
                
                if (isJumping && isGrounded)
                {
                    isGrounded = false;
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
                if (horizontalMove > 0 && !isFacingRight) FlipSprite();
                else if (horizontalMove < 0 && isFacingRight) FlipSprite();
            }
            else
            {
                if (onWall)
                {
                    HandleWallSliding();
                }
                else
                {
                    if (isJumping)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    }
                    else if (!isJumping && !isGrounded)
                    {
                        HandleFallGravity();
                    }
                }
            }
        }
    }
    private void HandleFallGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallSpeedMultiplier - 1) * Time.fixedDeltaTime;
            Mathf.Clamp(rb.velocity.y, float.MinValue, -maxFallSpeedMultiplier);
        }
    }
    private void HandleWallSliding()
    {
        if (!isWallSliding)
        {
            isWallSliding = true;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            rb.velocity = new Vector2(transform.localScale.x, slideSpeed);
            if (wallJump)
            {
                ExecuteWallJump();
            }
        }
    }
    private void ExecuteWallJump()
    {
        isJumping = true;
        wallJump = false;
        isWallSliding = false;
        FlipSprite();
        GetComponent<SpriteRenderer>().flipX = false;
        rb.velocity = new Vector2(transform.localScale.x * wallJumpXForce, wallJumpYForce);
    }
    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}