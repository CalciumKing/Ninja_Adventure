using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour, ICollisionHandler
{
    [SerializeField] float knockBackForce;
    [SerializeField] Transform possumDeathEffect;
    [SerializeField] int health;
    [SerializeField] int power;

    private Rigidbody2D rb;
    private PossumMovement pm;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PossumMovement>();
    }
    public int HandleDamagePlayer(Transform other)
    {
        Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();
        var knockBackDirection = (other.transform.position - transform.position).normalized;
        otherRB.AddForce(new Vector2(knockBackDirection.x * knockBackForce, knockBackForce / 2), ForceMode2D.Impulse);
        rb.AddForce(new Vector2(-knockBackDirection.x * knockBackForce, knockBackForce / 2), ForceMode2D.Impulse);
        return power;
    }
    public void HandleTakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(pm.Stun(2f));
        }
    }
    private void Die()
    {
        var deathEffect = Instantiate(possumDeathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffect.gameObject, .5f);
        Destroy(this.gameObject);
    }
}