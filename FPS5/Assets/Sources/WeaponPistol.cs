using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPistol : WeaponSystem
{

    [Header("Fire Effect")]
    [SerializeField]
    private GameObject muzzleFlash;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;
    [SerializeField]
    private Transform bulletSpawnPoint;

    [Header("AudioClip")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;
    [SerializeField]
    private AudioClip audioClipFire;
    [SerializeField]
    private AudioClip[] audioClipReload;
    [SerializeField]
    private AudioClip audioClipAim;

    public bool isModChange = false;
    private CasingMemoryPool casingMemoryPool;
    private ImpactMemoryPool impactMemoryPool;
    private PlayerMovement playerMovement;
    private ThrowGrenade throwGrenade;
    private RotateToMouse rotateToMouse;
    private PlayerController playerController;

    [SerializeField]
    private WeaponKnifeCollider weaponKnifeCollider;

    private void Awake()
    {
        base.Setup();
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        throwGrenade = GetComponentInChildren<ThrowGrenade>();
        rotateToMouse = GetComponentInParent<RotateToMouse>();
        playerController = GetComponentInParent<PlayerController>();
        mainCamera = Camera.main;
        weaponStatus.currentAmmo = weaponStatus.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        muzzleFlash.SetActive(false);
        Camera.main.fieldOfView = defaultFOV;
        ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
        ResetVariables();
    }

    private void ResetVariables()
    {
        isModChange = false;
        isAttack = false;
        isReload = false;
    }

    public override void StartGrenadeAction(int type = 0)
    {
        if (isGrenadeThrow == true) return;
        if (playerController.grenadeAmmo <= 0) return;
        StartCoroutine("OnGrenadeThrow", type);
    }

    private IEnumerator OnGrenadeThrow(int type)
    {
        isGrenadeThrow = true;
        playerController.grenadeAmmo--;
        grenadeAmmoEvent.Invoke(playerController.grenadeAmmo);
        animatorController.Play("PistolGrenadeThrow", -1, 0);
        throwGrenade.SetUp();
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animatorController.CurrentAnimationIs("Movement"))
            {
                isGrenadeThrow = false;

                yield break;
            }
            yield return null;
        }
        //PlaySound(audioClipFire);
    }

    public override void StartKnifeAction(int type = 0)
    {
        if (isKnifeAttack == true) return;
        StartCoroutine("OnKnifeAttack", type);
    }

    private IEnumerator OnKnifeAttack(int type)
    {
        weaponKnifeCollider.weapons = 1;
        StartWeaponKnifeCollider();
        isKnifeAttack = true;

        animatorController.SetFloat("attackType", type);

        animatorController.Play("PistolKnife", -1, 0);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animatorController.CurrentAnimationIs("Movement"))
            {
                isKnifeAttack = false;

                yield break;
            }
            yield return null;
        }
        //PlaySound(audioClipFire);
    }
    public override void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return;
        if (isModChange == true) return;

        if (type == 0)
        {
            OnAttack();
        }
        else if (type == 1)
        {
            if (isAttack == true) return;
            if (playerMovement.MoveSpeed >= 5.0f) return;
            StartCoroutine("AimModeChange");
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

    private IEnumerator AimModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.1f;

        animatorController.AimModeIs = !animatorController.AimModeIs;
        crossHairImage.enabled = !crossHairImage.enabled;

        float start = mainCamera.fieldOfView;
        float end = animatorController.AimModeIs == true ? aimFOV : defaultFOV;

        if (end == aimFOV)
        {
            rotateToMouse.rotCamXAxisSpeed = 1.5f;
            rotateToMouse.rotCamYAxisSpeed = 0.75f;
        }
        else if (end == defaultFOV)
        {
            rotateToMouse.rotCamXAxisSpeed = 4;
            rotateToMouse.rotCamYAxisSpeed = 2;
        }

        isModChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);
            yield return null;
        }
        isModChange = false;
    }

    public override void StartReload()
    {
        if (isReload == true || weaponStatus.currentAmmo >= weaponStatus.maxAmmo || weaponStatus.maxCurrentAmmo <= 0) return;
        isModChange = false;
        StopWeaponAction();
        if (animatorController.AimModeIs == true)
        {
            mainCamera.fieldOfView = defaultFOV;
            animatorController.AimModeIs = false;
            crossHairImage.enabled = !crossHairImage.enabled;
        }
        StartCoroutine("OnReload");
        StartCoroutine("ReloadMode", weaponStatus.currentAmmo);
    }

    private IEnumerator ReloadMode(int ammo)
    {
        if (ammo <= 0)
        {
            animatorController.ReloadType = 0;
            animatorController.OnReload();
            animatorController.Play("Reload", -1, 0);
            PlaySound(audioClipReload[0]);
        }
        else if (ammo > 0)
        {
            animatorController.ReloadType = 1;
            animatorController.OnReload();
            animatorController.Play("Reload", -1, 0);
            PlaySound(audioClipReload[1]);
        }
        yield return null;
    }

    private IEnumerator OnReload()
    {
        isReload = true;
        ReloadMode(weaponStatus.currentAmmo);

        if (weaponStatus.currentAmmo == 20 && weaponStatus.firedAmmo == 0)
        {
            weaponStatus.firedAmmo = 1;
        }

        while (true)
        {
            if (audioSource.isPlaying == false && animatorController.CurrentAnimationIs("Movement"))
            {
                isReload = false;
                if (weaponStatus.maxCurrentAmmo + weaponStatus.currentAmmo < weaponStatus.maxAmmo)
                {
                    weaponStatus.maxAmmo = weaponStatus.maxCurrentAmmo;
                    weaponStatus.currentAmmo += weaponStatus.maxAmmo;
                }
                else
                {
                    if (weaponStatus.currentAmmo > 0) weaponStatus.currentAmmo = weaponStatus.maxAmmo;
                    else if (weaponStatus.currentAmmo <= 0)
                    {
                        weaponStatus.currentAmmo = weaponStatus.maxAmmo - 1;
                        weaponStatus.firedAmmo--;
                    }
                }
                weaponStatus.maxCurrentAmmo -= weaponStatus.firedAmmo;
                if (weaponStatus.maxCurrentAmmo < 0) weaponStatus.maxCurrentAmmo = 0;
                ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
                weaponStatus.firedAmmo = 0;
                weaponStatus.maxAmmo = 21;
                yield break;
            }
            yield return null;
        }
    }

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

            if (weaponStatus.currentAmmo <= 0)
            {
                return;
            }
            weaponStatus.currentAmmo--;
            weaponStatus.firedAmmo++;
            ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
            string animation = animatorController.AimModeIs == true ? "AimFire" : "Fire";
            animatorController.Play(animation, -1, 0);

            StartCoroutine("OnMuzzleFlash");
            PlaySound(audioClipFire);

            if (!isCrouch)
            {
                rotateToMouse.eulerAngleX -= 1.0f;
                rotateToMouse.eulerAngleY += 0.2f;
            }
            else if(isCrouch)
            {
                rotateToMouse.eulerAngleX -= 0.5f;
                rotateToMouse.eulerAngleY += 0.1f;
            }

            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            TwoStepRayCast();
        }
    }

    private void TwoStepRayCast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        if (Physics.Raycast(ray, out hit, weaponStatus.attackDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * weaponStatus.attackDistance;
        }

        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponStatus.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.collider.CompareTag("Enemy"))
            {
                hit.rigidbody.AddForceAtPosition(new Vector3(10f, 0, 0), hit.transform.position);
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponStatus.damage);
            }
            else if (hit.transform.CompareTag("ExplosiveObject"))
            {
                hit.transform.GetComponent<ExplosiveObject>().TakeDamage(weaponStatus.damage);
            }
            else if (hit.collider.CompareTag("EnemyHead"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponStatus.damage + 30);
            }
        }
    }

    private IEnumerator OnMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(weaponStatus.attackRate * 0.3f);
        muzzleFlash.SetActive(false);
    }

    public override void IncreaseMainAmmo(int ammoAmount)
    {
        /*weaponStatus.maxCurrentAmmo += ammoAmount;
        playerController.grenadeAmmo += 1;
        ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
        grenadeAmmoEvent.Invoke(playerController.grenadeAmmo);*/
    }

    public override void IncreaseSubAmmo(int ammoAmount)
    {
        weaponStatus.maxCurrentAmmo += ammoAmount;
        playerController.grenadeAmmo += 1;
        ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
        grenadeAmmoEvent.Invoke(playerController.grenadeAmmo);
    }

    public void StartWeaponKnifeCollider()
    {
        weaponKnifeCollider.StartCollider(weaponStatus.knifeDamage);
    }
}
