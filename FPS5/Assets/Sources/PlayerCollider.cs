using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealingGel"))
        {
            other.GetComponent<ItemHealingGel>().useHealingGel(transform.parent.gameObject);
        }

        if (other.CompareTag("AmmoSupply"))
        {
            other.GetComponent<AmmoSupplyObject>().useAmmoSupply(transform.parent.GetChild(0).GetChild(0).gameObject);
            other.GetComponent<AmmoSupplyObject>().useAmmoSupply(transform.parent.GetChild(0).GetChild(1).gameObject);
        }
    }
}
