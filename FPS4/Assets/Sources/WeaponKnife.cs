using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnife : WeaponSystem
{
    [Header("AudioClip")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;
    [SerializeField]
    private AudioClip audioClipFire;

    public bool isModChange = false;

    private CasingMemoryPool casingMemoryPool;
    private ImpactMemoryPool impactMemoryPool;
    private PlayerMovement playerMovement;


    private void Awake()
    {
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        mainCamera = Camera.main;

    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        isAttack = false;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return;
        if (isModChange == true) return;

        if (type == 0 || type == 1)
        {
            if (weaponStatus.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            else
            {
                OnAttack();
            }
        }
    }

    public override void StopWeaponAction(int type = 0)
    {

        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public override void StartReload(){}

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        isAttack = true;
        if (Time.time - lastAttackTime > weaponStatus.attackRate)
        {
            if (animatorController.MoveSpeed > 0.5f)
            {
                return;
            }

            lastAttackTime = Time.time;

            ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
            string animation = animatorController.AttackType == true ? "LeftAttack" : "RightAttack";
            animatorController.Play(animation, -1, 0);

            PlaySound(audioClipFire);
        }
    }
}
