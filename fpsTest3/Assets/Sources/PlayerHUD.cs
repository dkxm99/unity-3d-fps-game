using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssultRifle weapon;
    [SerializeField]
    private PlayerStatus status;

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;
    [SerializeField]
    private Image imageWeaponIcon;
    [SerializeField]
    private Sprite[] spriteWeaponIcons;

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;
    [SerializeField]
    private Image imageBloodScreen;
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        SetupWeapon();
        weapon.ammoEvent.AddListener(UpdateAmmoHUD);
        status.hpEvent.AddListener(UpdateHpHUD);
    }
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.weaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.weaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int currentMaxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size=30>{currentMaxAmmo}";
        if(currentAmmo < 10)
        {
            textAmmo.text = $"<size=40> <color=#ff0000>{currentAmmo}<color=#000000ff>/</size=30>{currentMaxAmmo}";
        }
    }

    private void UpdateHpHUD(int previous, int current)
    {
        textHP.text = "HP" + current;

        if(previous <= current)
        {
            /*StopCoroutine("OnHealScreen");
            StartCoroutine("OnHealScreen");*/
            return;
        }

        if(previous - current > 0)
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

        while(percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
