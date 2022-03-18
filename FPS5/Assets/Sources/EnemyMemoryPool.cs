using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private GameObject enemySpawnPointPrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float enemySpawnTime = 10;
    [SerializeField]
    private float enemySpawnLatency = 1;

    private MemoryPool spawnPointMemoryPool;
    private MemoryPool enemyMemoryPool;

    private int EnemySpawnedAtOnce = 1;
    private Vector2Int mapSize = new Vector2Int(100, 100);

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnObject");
    }

    private IEnumerator SpawnObject()
    {
        int currentNum = 0;
        int maximumNum = 10;

        while (true)
        {
            for (int i = 0; i < EnemySpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();
                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 5,
                                                      Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));
                RaycastHit hit;
                Vector3 targetPoint = Vector3.zero;

                if (Physics.Raycast(item.transform.position, item.transform.forward, out hit, 15.0f))
                {
                    targetPoint = hit.point;
                    item.transform.position = targetPoint;
                    //Debug.DrawRay(item.transform.position, item.transform.forward * hit.distance, Color.red);
                }

                StartCoroutine("SpawnEnemy", item);
            }

            currentNum++;

            if (currentNum >= maximumNum)
            {
                currentNum = 0;
                EnemySpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;
        //item.GetComponent<EnemyFSM>().Setup(target);
        item.GetComponent<EnemyFSM>().Setup(target, this);
        //item.GetComponent<EnemyFSM>().Awake();

        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        spawnPointMemoryPool.DeactivatePoolItem(enemy);
        Destroy(enemy, 3);
    }
}
