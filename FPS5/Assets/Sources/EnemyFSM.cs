using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, Hit, }
public class EnemyFSM : MonoBehaviour
{
    [Header("Fire Effect")]
    [SerializeField]
    private GameObject muzzleFlash;

    /*[Header("target")]
    [SerializeField]
    private GameObject target;*/

    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognizeRange = 15;
    [SerializeField]
    private float pursuitMaxRange = 20;

    [Header("Attack")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float attackRate = 0.5f;

    [Header("Enemy Sight Range")]
    [SerializeField]
    private Collider sightCollider;

    private EnemyState enemyState = EnemyState.None;
    private float lastAttackTime = 0;
    private float fieldOfView = 120f;
    public Transform enemyHead;
    public Vector3 targetPos;

    private PlayerStatus status;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private ImpactMemoryPool impactMemoryPool;
    //private EnemyMemoryPool enemyMemoryPool;
    //private GameObject target;
    private Rigidbody rigid;
    public bool isDie = false;

    [Header("Target")]
    [SerializeField]
    private GameObject target;

     public void Awake()
     {
        muzzleFlash.SetActive(false);
        status = GetComponent<PlayerStatus>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        enemyHead = GetComponentInChildren<Transform>();
        rigid = GetComponent<Rigidbody>();
        navMeshAgent.updateRotation = false;
        rigid.isKinematic = true;

        //this.target = target;
        //this.enemyMemoryPool = enemyMemoryPool;
     }

    /*public void Setup(GameObject target, EnemyMemoryPool enemyMemoryPool)
    {
        muzzleFlash.SetActive(false);
        status = GetComponent<PlayerStatus>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        col = GetComponentInChildren<Transform>();
        rigid = GetComponent<Rigidbody>();
        navMeshAgent.updateRotation = false;
        rigid.isKinematic = true;
        collider = new Collider();
        this.target = target;
        this.enemyMemoryPool = enemyMemoryPool;
    }*/

    private void OnEnable()
    {
        ChangeState(EnemyState.Idle);
    }

    private void OnDisable()
    {
        StopCoroutine(enemyState.ToString());
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        StopCoroutine(enemyState.ToString());
        enemyState = newState;
        StartCoroutine(enemyState.ToString());
    }

    private IEnumerator Idle()
    {
        StartCoroutine("IdleToWander");

        while (true)
        {
            // animatorController.MoveSpeed = 0.0f;

            //animator.SetFloat("MoveSpeed", 0.0f);

            /*if (animatorController.EnemyMovement != 0)
            {
                animatorController.EnemyMovement = 0;
            }*/
            //StateByTargetDistance();
            yield return null;
        }
    }

    private IEnumerator IdleToWander()
    {
        int changeTime = Random.Range(1, 5);
        yield return new WaitForSeconds(changeTime);
        ChangeState(EnemyState.Wander);
    }

    private IEnumerator Wander()
    {
        float currentTime = 0;
        float maxTime = 10;

        navMeshAgent.speed = status.WalkSpeed;
        navMeshAgent.SetDestination(CalculateWanderPosition());

        Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.LookRotation(to - from);
        while (true)
        {
            // animatorController.MoveSpeed = 0.7f;
            animator.SetFloat("MoveSpeed", 0.3f);
            currentTime += Time.deltaTime;

            to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);

            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                ChangeState(EnemyState.Idle);
            }
            //StateByTargetDistance();
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
    {
        float wanderRadius = 10;
        int wanderJitter = 0;
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;

        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Pursuit()
    {
        while (true)
        {
            // animatorController.MoveSpeed = 1.0f;
            animator.SetFloat("MoveSpeed", 0.5f);
            navMeshAgent.speed = status.RunSpeed;
            navMeshAgent.SetDestination(target.transform.position);
            LookTarget();
            //StateByTargetDistance(); 
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        navMeshAgent.ResetPath();

        while (true)
        {
            LookTarget();
            //StateByTargetDistance();
            animator.SetFloat("MoveSpeed", 1.0f);
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;
                StartCoroutine("OnMuzzleFlash");
                GameObject clone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                clone.GetComponent<EnemyBullet>().SetUp(target.transform.position);
            }
            yield return null;
        }
    }

    private IEnumerator OnMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(attackRate * 0.3f);
        muzzleFlash.SetActive(false);
    }

    private void LookTarget()
    {
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        Quaternion rotation = Quaternion.LookRotation(to - from);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.05f);
    }

    /* private void StateByTargetDistance()
     {
         if (target == null) return;

         //RaycastHit hit;
         //Vector3 direction = (target.transform.position - transform.position).normalized;

         float distance = Vector3.Distance(target.transform.position, transform.position);
         //Debug.Log(distance);
         if (distance <= attackRange)
         {
             if (Physics.Raycast(transform.position + transform.up, direction, out hit, attackRange))
             {
                 if (hit.transform.CompareTag("Player"))
                 {
                     ChangeState(EnemyState.Attack);
                 }
             }
             //ChangeState(EnemyState.Attack);
         }
         else if (distance >= pursuitMaxRange)
         {
             ChangeState(EnemyState.Wander);
         }
         else if(distance < pursuitMaxRange)
         {
             ChangeState(EnemyState.Pursuit);
         }
     }*/

    private void OnTriggerStay(Collider other)
    {
        if(isDie == true)
        {
            return;
        }
        float distance = Vector3.Distance(target.transform.position, transform.position);

        if (distance >= targetRecognizeRange)
        {
            ChangeState(EnemyState.Wander);
        }

        if (other.gameObject == target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < fieldOfView * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, 30.0f))
                {
                    if (hit.collider.gameObject.name == "PlayerCollider")
                    {
                        Debug.DrawRay(transform.position + transform.up, transform.forward * hit.distance, Color.red);
                        if (distance < attackRange)
                        {
                            ChangeState(EnemyState.Attack);
                        }
                        else if (distance <= pursuitMaxRange && distance > attackRange)
                        {
                            ChangeState(EnemyState.Pursuit);
                        }
                    }
                }
            }
        }
    }

    private void ChangeRagdoll()
    {
        animator.enabled = false;       
        StopAllCoroutines();
        sightCollider.enabled = false;
        navMeshAgent.enabled = false;
        muzzleFlash.SetActive(false);
        rigid.isKinematic = false;
    }
    public void TakeDamage(int damage)
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        isDie = status.DecreaseHP(damage);
        if (distance > pursuitMaxRange)
        {
            //ChangeState(EnemyState.Pursuit);
            StartCoroutine("PursuitAndAttack");
        }
        /*else if(distance < attackRange)
        {
            ChangeState(EnemyState.Attack);
        }*/
        if(isDie == true)
        {
            //gameObject.SetActive(false); // 메모리풀
            ChangeRagdoll();
            //enemyMemoryPool.DeactivateEnemy(gameObject); //메모리풀
            StartCoroutine("Deactive");
        }
    }
   
    private IEnumerator PursuitAndAttack()
    {
        ChangeState(EnemyState.Pursuit);
        yield return new WaitForSeconds(2);
        ChangeState(EnemyState.Attack);
    }

    private IEnumerator Deactive()
    {
        transform.Find("Circle").gameObject.SetActive(false);
        yield return new WaitForSeconds(4);
        Destroy(gameObject);      
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetRecognizeRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pursuitMaxRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }*/
    /*private void OnDrawGizmos()
    {
        Handles.color = Color.black;
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, fieldOfView / 2, attackRange);
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, -fieldOfView / 2, attackRange);
    }*/
}