using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSupplyObject : MonoBehaviour
{
    [SerializeField]
    private int ammoAmount = 30;
    [SerializeField]
    private float moveDistance = 0.2f;
    [SerializeField]
    private float pingpongSpeed = 0.5f;
    [SerializeField]
    private float rotateSpeed = 50;

    private WeaponStatus weaponStatus;
    private WeaponAssultRifle weaponAssultRifle;

    private void Awake()
    {
        weaponAssultRifle = GetComponent<WeaponAssultRifle>();
    }

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while (true)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    public void useAmmoSupply(GameObject entity)
    {
        entity.GetComponent<WeaponAssultRifle>().IncreaseAmmo(ammoAmount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("supplied");
            weaponAssultRifle.IncreaseAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }

}
