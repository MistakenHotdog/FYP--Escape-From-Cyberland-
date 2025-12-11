using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform patrolPointA;
    public Transform patrolPointB;
    public string playerTag = "Player";

    // Movement speeds
    public float walkSpeed = 2f;
    public float runSpeed = 4.5f;

    // Player detection and shooting
    public float detectionRange = 15f;
    public float shootRange = 10f;
    public float stoppingDistance = 9f;
    public float fireRate = 0.6f;
    public float turnSpeed = 6f;

    public Transform muzzle;
    public float shootDamage = 10f;
    public LayerMask shootLayerMask = ~0;

    [Header("Muzzle & Audio")]
    public ParticleSystem muzzleFlash; // assign a ParticleSystem prefab or child
    public AudioClip shootSFX;
    private AudioSource audioSource;

    // Patrol timing
    public float idleTimeAtPatrolPoint = 4f;

    // Animator parameter names
    private const string ANIM_SPEED = "Speed";
    private const string ANIM_IS_SHOOTING = "IsShooting";
    private const string ANIM_TURN_TRIGGER = "TurnTrigger";
    private const string ANIM_IDLE_TRIGGER = "IdleTrigger";

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private enum AIState { Patrol, Approach, Shooting }
    private AIState state = AIState.Patrol;

    private Transform currentPatrolTarget;
    private bool patrollingForward = true;
    private float lastFireTime = 0f;
    private bool isWaitingAtPoint = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        if (patrolPointA == null || patrolPointB == null)
        {
            Debug.LogError("EnemyAI: Missing patrol points.");
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;

        currentPatrolTarget = patrolPointA;

        agent.speed = walkSpeed;
        agent.stoppingDistance = 0f;
        agent.SetDestination(currentPatrolTarget.position);
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= detectionRange)
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

    // PATROL
    void HandlePatrol()
    {
        animator.SetBool(ANIM_IS_SHOOTING, false);
        agent.speed = walkSpeed;
        agent.isStopped = false;
        agent.stoppingDistance = 0f;

        if (isWaitingAtPoint) return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            StartCoroutine(PatrolIdleSequence());
        }
    }

    IEnumerator PatrolIdleSequence()
    {
        isWaitingAtPoint = true;

        agent.isStopped = true;

        // Go to idle animation
        animator.SetTrigger(ANIM_IDLE_TRIGGER);

        yield return new WaitForSeconds(idleTimeAtPatrolPoint);

        // Turn animation before walking again
        animator.SetTrigger(ANIM_TURN_TRIGGER);

        yield return new WaitForSeconds(0.5f);

        patrollingForward = !patrollingForward;
        currentPatrolTarget = patrollingForward ? patrolPointA : patrolPointB;

        agent.isStopped = false;
        agent.SetDestination(currentPatrolTarget.position);

        isWaitingAtPoint = false;
    }

    // APPROACH PLAYER
    void HandleApproach()
    {
        animator.SetBool(ANIM_IS_SHOOTING, false);
        agent.speed = runSpeed;
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 targetPos = player.position - dir * (shootRange * 0.9f);

        agent.SetDestination(targetPos);

        float angle = Vector3.Angle(transform.forward, player.position - transform.position);
        if (angle > 60f)
            animator.SetTrigger(ANIM_TURN_TRIGGER);
    }

    // SHOOTING
    void HandleShooting()
    {
        agent.isStopped = true;
        agent.speed = 0f;

        animator.SetBool(ANIM_IS_SHOOTING, true);

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
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

        if (muzzleFlash != null)
        {
            // If muzzleFlash is a prefab instance, Play(); if it's a ParticleSystem child, Play() works
            muzzleFlash.Play();
        }

        // Play shooting sound
        if (shootSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSFX);
        }

        Vector3 origin = muzzle ? muzzle.position : transform.position + Vector3.up * 1.5f;
        Vector3 dir = (player.position + Vector3.up * 1f) - origin;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir.normalized, out hit, shootRange, shootLayerMask))
        {
            // Damage the player if hit
            PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(shootDamage);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
