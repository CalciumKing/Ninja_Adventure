using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gem : MonoBehaviour, ICollectable
{
    [SerializeField] int worth;
    public int Worth { get { return worth; } }
    public int Collect()
    {
        print("Gem Collected");
        Destroy(this.gameObject);
        return worth;
    }
}