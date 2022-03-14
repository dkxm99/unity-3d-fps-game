using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectByTimeObject : MonoBehaviour
{
    private ParticleSystem particle;
    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
