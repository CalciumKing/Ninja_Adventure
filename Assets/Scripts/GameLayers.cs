using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    public static GameLayers i;

    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float combatCheckDistance;

    [SerializeField] LayerMask playerLayer;
    [SerializeField] float playerLayerInt;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float enemyLayerInt;

    private void Awake() { i = this; }
    private void Start()
    {
        PlayerLayerInt = LayerMaskToInt(playerLayer);
        EnemyLayerInt = LayerMaskToInt(enemyLayer);
    }
    public float GroundCheckDistance => groundCheckDistance;
    public LayerMask GroundLayer => groundLayer;
    public float CombatCheckDistance => combatCheckDistance;
    public LayerMask PlayerLayer => playerLayer;
    public int PlayerLayerInt;
    public LayerMask EnemyLayer => enemyLayer;
    public int EnemyLayerInt;
    private int LayerMaskToInt(LayerMask layerMask)
    {
        int maskValue = layerMask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((maskValue & (1 << i)) != 0)
            {
                return i;
            }
        }
        return -1;
    }
}