using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitchingSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerHUD playerHUD;

    [SerializeField]
    private WeaponSystem[] weapons;

    private WeaponSystem currentWeapon;
    private WeaponSystem previousWeapon;

    private void Awake()
    {
        playerHUD.SetupAllWeapon(weapons);

        for (int i = 0; i < weapons.Length; ++i)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (!Input.anyKeyDown) return;

        int inputIndex = 0;
        if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0) && inputIndex < 3)
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }
    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        if (weapons[(int)weaponType] == null)
        {
            return;
        }

        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        currentWeapon = weapons[(int)weaponType];

        if (currentWeapon == previousWeapon)
        {
            return;
        }

        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }
        currentWeapon.gameObject.SetActive(true);
    }
}
