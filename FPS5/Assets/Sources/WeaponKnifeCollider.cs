using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnifeCollider : MonoBehaviour
{
    [Header("Knife Info")]
    [SerializeField]
    private ImpactMemoryPool impactMemoryPool;
    [SerializeField]
    private Transform knifeTransform;

    private new Collider collider;
    private int damage;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    public void StartCollider(int damage)
    {
        this.damage = damage;
        collider.enabled = true;

        StartCoroutine("DisableByTime", 0.1f);
    }

    private IEnumerator DisableByTime(float time)
    {
        yield return new WaitForSeconds(time);

        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        impactMemoryPool.SpawnImpactKnife(other, knifeTransform);

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyFSM>().TakeDamage(damage);
        }
        else if (other.CompareTag("ExplosiveObject"))
        {
            other.GetComponent<ExplosiveObject>().TakeDamage(damage);
        }
    }
}
