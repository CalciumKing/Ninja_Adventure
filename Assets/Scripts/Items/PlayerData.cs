using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] int gemsCollected;
    private void Start()
    {
        UIManager.i.UpdateGems(gemsCollected);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent(out ICollectable collectable))
        {
            var amt = collectable.Collect();
            UpdateGems(amt);
        }
    }
    private void UpdateGems(int amt)
    {
        if(gemsCollected + amt > 0) gemsCollected += amt;
        else gemsCollected = 0;
        UIManager.i.UpdateGems(gemsCollected);
    }
}