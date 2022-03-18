using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnife : WeaponSystem
{
    /*[Header("AudioClip")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;
    [SerializeField]
    private AudioClip audioClipFire;*/

    [SerializeField]
    private WeaponKnifeCollider weaponKnifeCollider;

    private void OnEnable()
    {
        //PlaySound(audioClipTakeOutWeapon);
        isAttack = false;

        ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxAmmo);
    }

    private void Awake()
    {
        base.Setup();

        weaponStatus.currentAmmo = weaponStatus.maxAmmo;
    }


    public override void StartWeaponAction(int type = 0) { }

    public override void StartKnifeAction()
    {
        if (isAttack == true) return;

        if (weaponStatus.isAutomaticAttack == true)
        {
            StartCoroutine("OnAttackLoop");
        }
        else
        {
            StartCoroutine("OnAttack");
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
        StopCoroutine("OnAttackLoop");
    }

    public override void StartReload() { }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            yield return StartCoroutine("OnAttack");
        }
    }

    private IEnumerator OnAttack()
    {
        isAttack = true;

        animatorController.SetFloat("attackType", 0);

        animatorController.Play("Fire", -1, 0);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animatorController.CurrentAnimationIs("Movement"))
            {
                isAttack = false;

                yield break;
            }
            yield return null;
        }
        //PlaySound(audioClipFire);
    }

    public void StartWeaponKnifeCollider()
    {
        weaponKnifeCollider.StartCollider(weaponStatus.damage);
    }
}