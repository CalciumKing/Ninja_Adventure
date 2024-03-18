using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionHandler
{
    int HandleDamagePlayer(Transform other);
    void HandleTakeDamage(int damageAmt);
}