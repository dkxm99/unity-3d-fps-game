using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealingGel : MonoBehaviour
{
    [SerializeField]
    private int healPoint = 30;
    [SerializeField]
    private float moveDistance = 0.2f;
    [SerializeField]
    private float pingpongSpeed = 0.5f;
    [SerializeField]
    private float rotateSpeed = 50;
    private PlayerStatus playerStatus;

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while(true)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    public void useHealingGel(GameObject entity)
    {
        entity.GetComponent<PlayerStatus>().IncreaseHP(healPoint);
        Destroy(gameObject);
    }

    private void Awake()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Healed");
            playerStatus.IncreaseHP(healPoint);
            Destroy(gameObject);
        }
    }
}
