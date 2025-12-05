using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform[] patrolPoints;
    public float chaseDistance = 6f;
    public float attackDistance = 1.6f;
    public float idleTime = 2f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("References")]
    public Animator animator;

    [HideInInspector] public StateMachine StateMachine;
    [HideInInspector] public NavMeshAgent Agent;

    private Transform player;

    // Animation parameter hashes (more efficient than strings)
    private readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private readonly int isRunningHash = Animator.StringToHash("isRunning");
    private readonly int isAttackingHash = Animator.StringToHash("isAttacking");
    private readonly int speedHash = Animator.StringToHash("speed");

    // States
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public AttackState attackState;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        // Get animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();

        StateMachine = new StateMachine();

        // Find player safely
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player not found! Make sure player has 'Player' tag.");

        // Create states
        idleState = new IdleState(this, idleTime);
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
    }

    private void Start()
    {
        StateMachine.Initialize(idleState);
    }

    private void Update()
    {
        StateMachine.Tick();
        UpdateAnimationSpeed();
    }

    // Animation Control Methods
    public void SetIdleAnimation()
    {
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isRunningHash, false);
    }

    public void SetWalkAnimation()
    {
        animator.SetBool(isWalkingHash, true);
        animator.SetBool(isRunningHash, false);
    }

    public void SetRunAnimation()
    {
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isRunningHash, true);
    }

    public void TriggerAttackAnimation()
    {
        animator.SetTrigger(isAttackingHash);
    }

    // Update speed parameter based on agent velocity
    private void UpdateAnimationSpeed()
    {
        if (Agent != null && animator != null)
        {
            float speed = Agent.velocity.magnitude;
            animator.SetFloat(speedHash, speed);
        }
    }

    // Helper Methods
    public Transform GetPlayer() => player;

    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }

    public bool IsPlayerInChaseRange()
    {
        return GetDistanceToPlayer() <= chaseDistance;
    }

    public bool IsPlayerInAttackRange()
    {
        return GetDistanceToPlayer() <= attackDistance;
    }

    // Visualization in editor
    private void OnDrawGizmosSelected()
    {
        // Chase range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // Patrol points
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                }
            }
        }
    }
}