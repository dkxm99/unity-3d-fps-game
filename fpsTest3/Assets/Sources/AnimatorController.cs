using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("movementSpeed", value);
        get => animator.GetFloat("movementSpeed");
    }

    public float ReloadType
    {
        set => animator.SetFloat("ReloadType", value);
        get => animator.GetFloat("ReloadType");
    }

    public bool AimModeIs
    {
        set => animator.SetBool("isAim", value);
        get => animator.GetBool("isAim");
    }

    public float EnemyMovement
    {
        set => animator.SetFloat("MoveSpeed", value);
        get => animator.GetFloat("MoveSpeed");
    }

    public void OnReload()
    {
        animator.SetTrigger("onReload");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}