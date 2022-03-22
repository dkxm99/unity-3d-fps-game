using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponName { AssaultRifle = 0, Pistol, Knife, Grenade }
[System.Serializable]
public struct WeaponStatus
{
    public WeaponName weaponName;
    public int damage;
    public int knifeDamage;
    public int firedAmmo;
    public int maxCurrentAmmo;
    public int currentAmmo;
    public int maxAmmo;
    public float attackRate;
    public float attackDistance;
    public bool isAutomaticAttack;
}