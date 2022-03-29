using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapMove : MonoBehaviour
{
    [Header("target")]
    [SerializeField]
    private GameObject player;
    private Vector3 v;

    private void Start()
    {
        v = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {       
        transform.position = player.transform.position + v;
    }
}
