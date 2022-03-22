using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }
public class PlayerStatus : MonoBehaviour
{
    [Header("Walk, Run Speed")]
    [SerializeField]
    public float walkSpeed;
    [SerializeField]
    public float runSpeed;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100;
    private int currentHP;
    
    [HideInInspector]
    public HPEvent hpEvent = new HPEvent();

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = currentHP;
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        hpEvent.Invoke(previousHP, currentHP);

        if (currentHP == 0)
        {
            return true;
        }
        return false;
    }
    public void IncreaseHP(int heal)
    {
        int previousHP = currentHP;
        currentHP = currentHP + heal > 100 ? 100 : currentHP + heal;

        hpEvent.Invoke(previousHP, currentHP);
    }
}
