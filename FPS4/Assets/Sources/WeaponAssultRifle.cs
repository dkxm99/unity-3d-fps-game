using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }*/
public class WeaponAssultRifle : WeaponSystem
{
    /*[HideInInspector]
    public AmmoEvent ammoEvent = new AmmoEvent();*/

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

    /*[Header("Weapon Setting")]
    [SerializeField]
    private WeaponStatus weaponStatus;*/

    /*[Header("AimUi")]
    [SerializeField]
    public Image aimImage;

    /*private float lastAttackTime = 0;
    private bool isReload = false;
    public bool isAttack = false;*/
    public bool isModChange = false;

    /*private AudioSource audioSource;
    private AnimatorController animatorController;*/
    private CasingMemoryPool casingMemoryPool;
    private ImpactMemoryPool impactMemoryPool;
    private PlayerMovement playerMovement;

    /*[HideInInspector]
    public float defaultFOV = 60;
    public float aimFOV = 30;
    [HideInInspector]
    public Camera mainCamera;*/

    //public WeaponName weaponName => weaponStatus.weaponName;

    private void Awake()
    {
        base.Setup();

        /*audioSource = GetComponent<AudioSource>();
        animatorController = GetComponentInParent<AnimatorController>();*/
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        mainCamera = Camera.main;

        weaponStatus.currentAmmo = weaponStatus.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        muzzleFlash.SetActive(false);
        ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
        ResetVariables();
    }

    private void ResetVariables()
    {
        isModChange = false;
        isAttack = false;
        isReload = false;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return;
        if (isModChange == true) return;

        if (type == 0)
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
        aimImage.enabled = !aimImage.enabled;

        float start = mainCamera.fieldOfView;
        float end = animatorController.AimModeIs == true ? aimFOV : defaultFOV;
       
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
            aimImage.enabled = !aimImage.enabled;
        }
        StartCoroutine("OnReload");
    }

    private void ReloadMode(int ammo)
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
    }

    private IEnumerator OnReload()
    {
        isReload = true;
        ReloadMode(weaponStatus.currentAmmo);

        if (weaponStatus.currentAmmo == 30 && weaponStatus.firedAmmo == 0)
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
                weaponStatus.maxAmmo = 31;
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

            if(hit.transform.CompareTag("Enemy"))
            {
                hit.rigidbody.AddForceAtPosition(new Vector3(10f, 0, 0), hit.transform.position);
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponStatus.damage);
            }
            else if(hit.transform.CompareTag("ExplosiveObject"))
            {
                hit.transform.GetComponent<ExplosiveObject>().TakeDamage(weaponStatus.damage);
            }
        }
    }

    private IEnumerator OnMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(weaponStatus.attackRate * 0.3f);
        muzzleFlash.SetActive(false);
    }

    /*private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }*/

    public void IncreaseAmmo(int ammoAmount)
    {
        weaponStatus.maxCurrentAmmo += ammoAmount;
        ammoEvent.Invoke(weaponStatus.currentAmmo, weaponStatus.maxCurrentAmmo);
    }
}
