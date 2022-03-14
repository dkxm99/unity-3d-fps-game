using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : ExplosiveObject
{
    [Header("Explosive Barrel")]
    [SerializeField]
    private GameObject explosionEffectPrefab;
    [SerializeField]
    private float explosionDelayTime = 0.3f;
    [SerializeField]
    private float explosionRadius = 7.5f;
    [SerializeField]
    private float explosionForce = 1000.0f;

    private bool isExplode = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP <= 0 && isExplode == false)
        {
            StartCoroutine("ExplodeBarrel");
        }
    }

    private IEnumerator ExplodeBarrel()
    {
        yield return new WaitForSeconds(explosionDelayTime);

        isExplode = true;

        Bounds bounds = GetComponent<Collider>().bounds;
        Instantiate(explosionEffectPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if(player != null)
            {
                player.TakeDamage(50);
                continue;
            }
            EnemyFSM enemy = hit.GetComponentInParent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(300);
                continue;
            }
            ExplosiveObject explosive = hit.GetComponent<ExplosiveObject>();
            if (explosive != null)
            {
                explosive.TakeDamage(300);
            }
            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
        Destroy(gameObject);
    }
}
