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

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Audio")]
    public AudioSource shootAudioSource;
    public AudioClip shootSound;

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
    private Coroutine patrolCoroutine;

    private const string ANIM_SPEED = "Speed";
    private const string ANIM_IS_SHOOTING = "IsShooting";
    private const string ANIM_TURN_TRIGGER = "TurnTrigger";
    private const string ANIM_IDLE_TRIGGER = "IdleTrigger";

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 🔊 Setup AudioSource
        if (shootAudioSource == null)
        {
            shootAudioSource = GetComponent<AudioSource>();
            if (shootAudioSource == null)
                shootAudioSource = gameObject.AddComponent<AudioSource>();
        }

        shootAudioSource.playOnAwake = false;
        shootAudioSource.spatialBlend = 1f;
    }

    void Start()
    {
        if (agent == null || animator == null)
        {
            Debug.LogWarning($"[EnemyAI] {name}: Missing NavMeshAgent or Animator.");
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;

        currentPatrolTarget = patrolPointA;

        if (currentPatrolTarget != null)
        {
            agent.speed = walkSpeed;
            agent.SetDestination(currentPatrolTarget.position);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float currentDetectionRange = isAlerted ? alertDetectionRange : normalDetectionRange;

        if (distToPlayer <= currentDetectionRange && HasLineOfSightToPlayer())
        {
            if (distToPlayer > shootRange)
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

    // ---------------- LINE OF SIGHT ----------------
    bool HasLineOfSightToPlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 target = player.position + Vector3.up;
        Vector3 dir = target - origin;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude))
        {
            return hit.collider.CompareTag(playerTag);
        }

        return true;
    }

    // ---------------- ALERT ----------------
    public void SetAlert(bool alert)
    {
        isAlerted = alert;
        if (agent != null)
            agent.speed = alert ? runSpeed : walkSpeed;
    }

    // ---------------- PATROL ----------------
    void HandlePatrol()
    {
        animator.SetBool(ANIM_IS_SHOOTING, false);
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.speed = isAlerted ? runSpeed : walkSpeed;

        if (isWaitingAtPoint) return;

        if (!agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            patrolCoroutine = StartCoroutine(PatrolIdleSequence());
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
        if (currentPatrolTarget != null)
            agent.SetDestination(currentPatrolTarget.position);

        isWaitingAtPoint = false;
        patrolCoroutine = null;
    }

    // ---------------- APPROACH ----------------
    void HandleApproach()
    {
        CancelPatrolIfRunning();

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
        CancelPatrolIfRunning();

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

    private void CancelPatrolIfRunning()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
            isWaitingAtPoint = false;
        }
    }

    // 🔫 BULLET SHOOTING
    IEnumerator FireSingleShot()
    {
        // 🔊 SOUND
        if (shootAudioSource != null && shootSound != null)
        {
            shootAudioSource.pitch = Random.Range(0.9f, 1.1f);
            shootAudioSource.PlayOneShot(shootSound);
        }

        yield return null;

        if (bulletPrefab != null && firePoint != null)
        {
            // Aim bullet toward player
            Vector3 targetPos = player.position + Vector3.up * 1.2f;
            Quaternion rot = Quaternion.LookRotation(targetPos - firePoint.position);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // FORCE direction toward player
            Vector3 direction = (player.position + Vector3.up - firePoint.position).normalized;
            bullet.transform.forward = direction;
        }
    }
}