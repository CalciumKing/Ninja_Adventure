using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PossumMovement : EnemyMovement
{
    private void FixedUpdate()
    {
        var player = CheckForPlayer();
        switch (currentState)
        {
            case EnemyState.PATROL:
                Patrol();
                if(player != null) SetChase(player);
            break;
            case EnemyState.CHASE:
                Chase();
                if (player == null)
                {
                    if (chaseTimer > 0) chaseTimer -= Time.deltaTime;
                    else SetReset();
                }
                else chaseTimer = chaseTime;
            break;
            case EnemyState.RESET:
                ResetEnemy();
            break;
            case EnemyState.STUN:
            break;
        }
        UpdateFacing();
    }
    private void Patrol()
    {
        if (reachDestination || !CheckForGround()) MoveToNextPoint();
        //Debug.DrawRay(edgeCheck.transform.position, Vector2.down * GameLayers.i.GroundCheckDistance, Color.red);
        reachDestination = Vector2.Distance(patrolTarget.position, transform.position) < .4f;
        var destPos = new Vector3(patrolTarget.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, destPos, currentSpeed * Time.fixedDeltaTime);
    }
    private void Chase()
    {
        var destPos = new Vector3(patrolTarget.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, destPos, currentSpeed * Time.fixedDeltaTime);
    }
    private void ResetEnemy()
    {
        if (reachDestination)
        {
            currentState = EnemyState.PATROL;
            currentSpeed = patrolSpeed;
            resetTarget = null;
        }
        reachDestination = Vector2.Distance(resetPosition, transform.position) < .4f;
        var destPos = new Vector3(resetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, destPos, currentSpeed * Time.fixedDeltaTime);
    }
    private void SetChase(GameObject chaseTarget)
    {
        resetTarget = patrolTarget;
        resetPosition = transform.position;
        currentState = EnemyState.CHASE;
    }
    private void MoveToNextPoint()
    {
        currentPoint = (currentPoint + 1) % points.Count;
        patrolTarget = points[currentPoint];
    }
}