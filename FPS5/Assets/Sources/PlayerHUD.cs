using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private PlayerStatus status;
    [SerializeField]
    private ManageGame manageGame;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;
    [SerializeField]
    private Image imageWeaponIcon;
    [SerializeField]
    private Sprite[] spriteWeaponIcons;
    [SerializeField]
    private Vector2[] weaponSizeIcons;

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;
    [Header("GrenadeAmmo")]
    [SerializeField]
    private TextMeshProUGUI textGrenadeAmmo;

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    [Header("EnemyCount")]
    [SerializeField]
    private TextMeshProUGUI textEnemyCount;

    private WeaponSystem weapon;

    private void Awake()
    {
        //SetupWeapon();
        status.hpEvent.AddListener(UpdateHpHUD);
        weapon.ammoEvent.AddListener(UpdateAmmoHUD);
        weapon.grenadeAmmoEvent.AddListener(UpdateGrenadeAmmoHUD);
        manageGame.enemyCountEvent.AddListener(UpdateEnemyCountHUD);
    }

    public void SetupAllWeapon(WeaponSystem[] weapons)
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            weapons[i].ammoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].grenadeAmmoEvent.AddListener(UpdateGrenadeAmmoHUD);
        }
    }
    
    public void SetupEnemyCount()
    {
        manageGame.enemyCountEvent.AddListener(UpdateEnemyCountHUD);
    }

    public void SwitchingWeapon(WeaponSystem newWeapon)
    {
        weapon = newWeapon;

        SetupWeapon();
    }
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = weaponSizeIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int currentMaxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size=30>{currentMaxAmmo}";
        if (currentAmmo < 10)
        {
            textAmmo.text = $"<size=40> <color=#ff0000>{currentAmmo}<color=#000000ff>/</size=30>{currentMaxAmmo}";
        }
    }

    private void UpdateGrenadeAmmoHUD(int grenadeAmmo)
    {
        textGrenadeAmmo.text = $"<size=30>{grenadeAmmo}";
    }

    private void UpdateEnemyCountHUD(int enemies)
    {
        textEnemyCount.text = $"Left: {enemies}";
    }

    private void UpdateHpHUD(int previous, int current)
    {      
        if(current <= 30)
        {
            textHP.text = $"HP <color=#ff0000>{current}";
        }
        else
        {
            textHP.text = "HP" + current;
        }

        if (previous <= current)
        {
            /*StopCoroutine("OnHealScreen");
            StartCoroutine("OnHealScreen");*/
            return;
        }

        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    /*private IEnumerator OnHealScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = Color.green;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }*/

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(0.2f, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}