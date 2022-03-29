using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType { Normal = 0, Enemy, }
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab;
    private MemoryPool[] memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpactKnife(Collider other, Transform knifeTransform)
    {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("ExplosiveObject"))
        {
            OnSpawnImpact(ImpactType.Normal, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.transform.CompareTag("Enemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Floor") || hit.transform.CompareTag("ExplosiveObject") || hit.collider.CompareTag("EnemyHead"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("Enemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);
    }
}
