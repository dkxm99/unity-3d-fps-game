using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExplosiveObject : MonoBehaviour
{
    [Header("Explosive Info")]
    [SerializeField]
    protected int maxHP = 100;
    protected int currentHP;
    private void Awake()
    {
        currentHP = maxHP;
    }

    public abstract void TakeDamage(int damage);
}
