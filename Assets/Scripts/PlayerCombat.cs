using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] bool wasHit;
    [SerializeField] float stunDuration = .5f;
    [SerializeField] float knockBackForce;

    [SerializeField] int power;
    [SerializeField] float bounceForce;

    private Rigidbody2D rb;
    private PlayerMovement pm;

    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UIManager.i.UpdateCherries(currentHealth);
    }
    private void TakeDamage(int amt)
    {
        currentHealth -= amt;
        UIManager.i.UpdateCherries(currentHealth);
        if (currentHealth <= 0) print("Youre dead!");
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out ICollisionHandler collisionHandler))
        {
            if (pm.CurrentJumpState == FoxJumpState.FALL)
            {
                StompEnemy();
                collisionHandler.HandleTakeDamage(power);
            }
            else
            {
                var dmg = collisionHandler.HandleDamagePlayer(transform);
                TakeDamage(dmg);
                StartCoroutine(PlayerStun(stunDuration));
            }
        }
    }
    private void StompEnemy()
    {
        rb.AddForce(new Vector2(rb.velocity.x, bounceForce), ForceMode2D.Impulse);
    }
    private IEnumerator PlayerStun(float duration)
    {
        pm.CanMove = false;
        yield return new WaitForSeconds(duration);
        pm.CanMove = true;
    }
}