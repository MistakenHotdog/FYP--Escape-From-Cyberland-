using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform patrolPointA;
    public Transform patrolPointB;
    public string playerTag = "Player";

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4.5f;

    [Header("Detection")]
    public float normalDetectionRange = 15f;
    public float alertDetectionRange = 30f;
    public float shootRange = 10f;
    public float stoppingDistance = 9f;

    [Header("Combat")]
    public float fireRate = 0.6f;
    public float turnSpeed = 6f;
    public Transform muzzle;
    public float shootDamage = 10f;
    public LayerMask shootLayerMask = ~0;

    [Header("Patrol")]
    public float idleTimeAtPatrolPoint = 4f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private bool isAlerted = false;

    private enum AIState { Patrol, Approach, Shooting }
    private AIState state = AIState.Patrol;

    private Transform currentPatrolTarget;
    private bool patrollingForward = true;
    private float lastFireTime = 0f;
    private bool isWaitingAtPoint = false;

    private const string ANIM_SPEED = "Speed";
    private const string ANIM_IS_SHOOTING = "IsShooting";
    private const string ANIM_TURN_TRIGGER = "TurnTrigger";
    private const string ANIM_IDLE_TRIGGER = "IdleTrigger";

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;

        currentPatrolTarget = patrolPointA;

        agent.speed = walkSpeed;
        agent.SetDestination(currentPatrolTarget.position);
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        float currentDetectionRange = isAlerted ? alertDetectionRange : normalDetectionRange;

        if (distToPlayer <= currentDetectionRange)
        {
            if (distToPlayer > shootRange * 1.1f)
                state = AIState.Approach;
            else
                state = AIState.Shooting;
        }
        else
        {
            state = AIState.Patrol;
        }

        switch (state)
        {
            case AIState.Patrol:
                HandlePatrol();
                break;

            case AIState.Approach:
                HandleApproach();
                break;

            case AIState.Shooting:
                HandleShooting();
                break;
        }

        animator.SetFloat(ANIM_SPEED, agent.velocity.magnitude);
    }

    // ---------------- ALERT SYSTEM ----------------
    public void SetAlert(bool alert)
    {
        isAlerted = alert;

        if (alert)
        {
            agent.speed = runSpeed;
            Debug.Log(name + " ALERTED");
        }
        else
        {
            agent.speed = walkSpeed;
            Debug.Log(name + " CALM");
        }
    }

    // ---------------- PATROL ----------------
    void HandlePatrol()
    {
        animator.SetBool(ANIM_IS_SHOOTING, false);
        agent.isStopped = false;
        agent.stoppingDistance = 0f;

        if (isWaitingAtPoint) return;

        if (!agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            StartCoroutine(PatrolIdleSequence());
        }
    }

    IEnumerator PatrolIdleSequence()
    {
        isWaitingAtPoint = true;

        agent.isStopped = true;
        animator.SetTrigger(ANIM_IDLE_TRIGGER);

        yield return new WaitForSeconds(idleTimeAtPatrolPoint);

        animator.SetTrigger(ANIM_TURN_TRIGGER);

        yield return new WaitForSeconds(0.5f);

        patrollingForward = !patrollingForward;
        currentPatrolTarget = patrollingForward ? patrolPointA : patrolPointB;

        agent.isStopped = false;
        agent.SetDestination(currentPatrolTarget.position);

        isWaitingAtPoint = false;
    }

    // ---------------- APPROACH ----------------
    void HandleApproach()
    {
        animator.SetBool(ANIM_IS_SHOOTING, false);
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
        agent.speed = runSpeed;

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 targetPos = player.position - dir * (shootRange * 0.9f);

        agent.SetDestination(targetPos);
    }

    // ---------------- SHOOTING ----------------
    void HandleShooting()
    {
        agent.isStopped = true;
        animator.SetBool(ANIM_IS_SHOOTING, true);

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * turnSpeed);
        }

        if (Time.time >= lastFireTime + 1f / fireRate)
        {
            lastFireTime = Time.time;
            StartCoroutine(FireSingleShot());
        }
    }

    IEnumerator FireSingleShot()
    {
        yield return null;

        Vector3 origin = muzzle ? muzzle.position : transform.position + Vector3.up * 1.5f;
        Vector3 dir = (player.position + Vector3.up) - origin;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, shootRange, shootLayerMask))
        {
            // Add damage here if needed
        }
    }
}