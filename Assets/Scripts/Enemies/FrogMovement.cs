using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;
public enum FrogJumpState
{
    IDLE = 0,
    JUMP = 1,
    FALL = 2,
}

public class FrogMovement : EnemyMovement
{
    [SerializeField] bool isCroakIdle = true;
    [SerializeField] bool isJumpIdle = true;
    [SerializeField] float jumpForceX;
    [SerializeField] float jumpForceY;

    [SerializeField] float jumpTimer;
    [SerializeField] float croakTime;
    [SerializeField] float croakTimer;
    [SerializeField] FrogJumpState frogJumpState;

    private float lastYPosition;
    Animator anim;
    Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    protected override void Start()
    {
        base.Start();
        currentPoint = 0;
        jumpTimer = currentSpeed;
        croakTimer = croakTime;
        lastYPosition = transform.position.y;
    }
    private void Update()
    {
        if (enemyState == EnemyState.PATROL) PlayCroakAnimation();
    }
    private void PlayCroakAnimation()
    {
        if (isCroakIdle && isJumpIdle && croakTimer > 0) croakTimer -= Time.deltaTime;
        if (croakTimer <= 0)
        {
            anim.SetTrigger("croak");
            croakTimer = Random.Range(croakTime - 1f, croakTime + 1.5f);
        }
    }
    private void FixedUpdate()
    {
        bool wasGrounded = CheckIfGrounded();
        if (isGrounded)
        {
            HandleLanding(wasGrounded);
            chaseTarget = CheckForPlayer();
        }
        else HandleAirState();
        switch(enemyState)
        {
            case EnemyState.PATROL:
                if (IsTimeToJump()) FrogPatrol();
                IsTimeToJump();
                if (chaseTarget != null)
                {
                    SetChase(chaseTarget.transform);
                    FrogChase();
                }
            break;
            case EnemyState.CHASE:
                if(IsTimeToJump()) FrogChase();
                if(chaseTarget == null && isGrounded)
                {
                    if(chaseTimer > 0) chaseTimer -= Time.deltaTime;
                    else SetReset();
                }
                else chaseTimer = chaseTime;
            break;
            case EnemyState.RESET:
                if(IsTimeToJump()) FrogReset();
            break;
            case EnemyState.STUN:

            break;
        }
        anim.SetInteger("jumpState", (int)frogJumpState);
        lastYPosition = transform.position.y;
    }
    private bool IsTimeToJump()
    {
        if (isJumpIdle && isCroakIdle)
        {
            jumpTimer -= Time.fixedDeltaTime;
            if (jumpTimer <= 0)
            {
                isJumpIdle = false;
                return true;
            }
        }
        return false;
    }
    private void FrogPatrol()
    {
        int dir = (patrolTarget.position.x < transform.position.x) ? -1 : 1;
        rb.AddForce(new Vector2(jumpForceX * dir, jumpForceY), ForceMode2D.Impulse);
    }
    private void FrogChase()
    {
        int dir = (patrolTarget.position.x < transform.position.x) ? -1 : 1;
        var chaseSpeedModifier = 1.5f;
        var chaseHeightModifier = 1.5f;
        rb.AddForce(new Vector2(jumpForceX * dir * chaseSpeedModifier, jumpForceY * chaseHeightModifier), ForceMode2D.Impulse);
    }
    private void FrogReset()
    {
        if (reachDestination) SetPatrol();
        reachDestination = Vector2.Distance(resetPosition, transform.position) < 2f;
        int dir = (patrolTarget.position.x < transform.position.x) ? -1 : 1;
        rb.AddForce(new Vector2(jumpForceX * dir, jumpForceY), ForceMode2D.Impulse);
    }
    private void ToggleCroak()
    {
        isCroakIdle = !isCroakIdle;
    }
    private void HandleLanding(bool wasGrounded)
    {
        if (!wasGrounded)
        {
            frogJumpState = FrogJumpState.IDLE;
            jumpTimer = currentSpeed;
            isJumpIdle = true;
            rb.velocity = Vector2.zero;
            UpdateFacing();
        }
    }
    private void HandleAirState()
    {
        if (transform.position.y > lastYPosition)
        {
            if (frogJumpState == FrogJumpState.IDLE)
            {
                frogJumpState = FrogJumpState.JUMP;
            }
        }
        else if (transform.position.y < lastYPosition)
        {
            if (frogJumpState == FrogJumpState.JUMP)
            {
                frogJumpState = FrogJumpState.FALL;
            }
        }
    }
}