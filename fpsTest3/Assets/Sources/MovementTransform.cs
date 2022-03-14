using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]
    private Transform target;

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        lookTarget();
    }

    private void lookTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
            Quaternion rotation = Quaternion.LookRotation(to - from);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.0f);
        }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
