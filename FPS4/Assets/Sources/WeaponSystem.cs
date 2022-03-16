using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType{Main = 0, Sub, Melee, Throw }

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

public abstract class WeaponSystem : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent ammoEvent = new AmmoEvent();

    [Header("WeaponSystem")]
    [SerializeField]
    protected WeaponType weaponType;
    [SerializeField]
    protected WeaponStatus weaponStatus;

    protected float lastAttackTime = 0;
    protected bool isReload = false;
    protected bool isAttack = false;
    protected AudioSource audioSource;
    protected AnimatorController animatorController;

    public AnimatorController AnimatorController => animatorController;
    public WeaponName WeaponName => weaponStatus.weaponName;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    protected void Setup()
    {
        audioSource = GetComponent<AudioSource>();
        animatorController = GetComponent<AnimatorController>();
    }
}
