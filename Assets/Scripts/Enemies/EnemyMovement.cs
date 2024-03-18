using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum EnemyState
{
    PATROL,
    CHASE,
    RESET,
    STUN,
}
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] protected EnemyState enemyState;
    [SerializeField] protected EnemyState currentState;

    [SerializeField] protected bool isGrounded;
    [SerializeField] protected bool reachDestination;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected Transform edgeCheck;
    [SerializeField] protected Transform patrolTarget;

    [SerializeField] protected float patrolSpeed;
    [SerializeField] protected float chaseSpeed;
    [SerializeField] protected float currentSpeed;

    [SerializeField] protected Transform resetTarget;
    [SerializeField] protected Vector2 resetPosition;
    [SerializeField] protected float chaseTime = .5f;
    [SerializeField] protected float chaseTimer;
    [SerializeField] protected Transform chaseTarget;

    [SerializeField] protected List<Transform> points;
    [SerializeField] protected int currentPoint;

    protected virtual void Start()
    {
        patrolTarget = points[currentPoint];
        currentSpeed = patrolSpeed;
    }
    protected bool CheckIfGrounded()
    {
        bool wasGrounded = isGrounded;
        Debug.DrawRay(groundCheck.position, Vector2.down * GameLayers.i.GroundCheckDistance, Color.red);
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, GameLayers.i.GroundCheckDistance, GameLayers.i.GroundLayer);
        return wasGrounded;
    }
    protected Transform CheckForPlayer()
    {
        //Debug.DrawRay(transform.position, Vector2.left  * transform.localScale.x * GameLayers.i.GroundCheckDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, GameLayers.i.CombatCheckDistance, GameLayers.i.PlayerLayer);
        if (hit) return hit.transform;
        return null;
    }
    protected bool CheckForGround()
    {
        return Physics2D.Raycast(edgeCheck.transform.position, Vector2.down, GameLayers.i.GroundCheckDistance, GameLayers.i.GroundLayer);
    }
    protected void UpdateFacing()
    {
        Vector3 facing = patrolTarget.transform.position - transform.position;
        if (facing.x > 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (facing.x < 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }
    public IEnumerator Stun(float duration)
    {
        var previousState = currentState;
        var previousSpeed = currentSpeed;
        currentState = EnemyState.STUN;
        currentSpeed = 0;
        Physics2D.IgnoreLayerCollision(GameLayers.i.PlayerLayerInt, GameLayers.i.EnemyLayerInt, true);
        yield return new WaitForSeconds(duration);
        Physics2D.IgnoreLayerCollision(GameLayers.i.PlayerLayerInt, GameLayers.i.EnemyLayerInt, false);
        currentSpeed = previousSpeed;
        currentState = previousState;
    }
    protected void SetPatrol()
    {
        enemyState = EnemyState.PATROL;
        currentSpeed = patrolSpeed;
        resetTarget = null;
    }
    protected void SetReset()
    {
        patrolTarget = resetTarget;
        enemyState = EnemyState.RESET;
    }
    protected void SetChase(Transform chaseTarget)
    {
        resetTarget = patrolTarget;
        resetPosition = transform.position;
        patrolTarget = chaseTarget.transform;
        currentSpeed = chaseSpeed;
        enemyState = EnemyState.CHASE;
        chaseTimer = chaseTime;
    }
}