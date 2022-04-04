using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    [Header("Grenade Spawn Point")]
    [SerializeField]
    private Transform spawnPoint;
    [Header("GrenadePrefab")]
    [SerializeField]
    private Transform grenade;


    public void SetUp()
    {
        StartCoroutine("SpawnGrenade");
    }
    private IEnumerator SpawnGrenade()
    {       
        yield return new WaitForSeconds(1f);  
        Instantiate(grenade, spawnPoint.position, Quaternion.identity);  
    }
}
