using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType { Main = 0, Sub, Melee, Throw }

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class GrenadeAmmoEvent : UnityEngine.Events.UnityEvent<int> { }

public abstract class WeaponSystem : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent ammoEvent = new AmmoEvent();
    [HideInInspector]
    public GrenadeAmmoEvent grenadeAmmoEvent = new GrenadeAmmoEvent();

    [Header("WeaponSystem")]
    [SerializeField]
    protected WeaponType weaponType;
    [SerializeField]
    protected WeaponStatus weaponStatus;

    [HideInInspector]
    public float defaultFOV = 60;
    public float aimFOV = 30;
    [HideInInspector]
    public Camera mainCamera;


    [Header("AimUi")]
    [SerializeField]
    public Image crossHairImage;

    protected float lastAttackTime = 0;
    protected bool isReload = false;
    public bool isCrouch = false;
    protected bool isAttack = false;
    protected bool isKnifeAttack = false;
    protected bool isGrenadeThrow = false;
    protected AudioSource audioSource;
    protected AnimatorController animatorController;

    public AnimatorController AnimatorController => animatorController;
    public WeaponName WeaponName => weaponStatus.weaponName;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();
    public abstract void StartKnifeAction(int type = 0);

    public abstract void StartGrenadeAction(int type = 0);
    public abstract void IncreaseMainAmmo(int ammoAmount);

    public abstract void IncreaseSubAmmo(int ammoAmount);

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
