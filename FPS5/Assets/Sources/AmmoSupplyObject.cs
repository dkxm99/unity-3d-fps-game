using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSupplyObject : MonoBehaviour
{
    [SerializeField]
    private int ammoAmount = 60;
    [SerializeField]
    private int subAmmoAmount = 40;
    [SerializeField]
    private float moveDistance = 0.2f;
    [SerializeField]
    private float pingpongSpeed = 0.5f;
    [SerializeField]
    private float rotateSpeed = 50;

    private WeaponSystem weaponSystem;

    private void Awake()
    {
        weaponSystem = GetComponent<WeaponSystem>();
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
        entity.GetComponent<WeaponSystem>().IncreaseMainAmmo(ammoAmount);
        entity.GetComponent<WeaponSystem>().IncreaseSubAmmo(subAmmoAmount);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("supplied");
            weaponSystem.IncreaseMainAmmo(ammoAmount);
            weaponSystem.IncreaseSubAmmo(subAmmoAmount);
            Destroy(gameObject);
        }
    }

}
